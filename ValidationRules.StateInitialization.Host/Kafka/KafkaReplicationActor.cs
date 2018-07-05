using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using NuClear.Messaging.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.StateInitialization.Core.Storage;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Commands;

using Polly;

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    internal sealed class KafkaReplicationActor : IActor
    {
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IDataObjectTypesProviderFactory _dataObjectTypesProviderFactory;
        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;
        private readonly KafkaMessageFlowInfoProvider _kafkaMessageFlowInfoProvider;
        private readonly IReadOnlyCollection<IBulkCommandFactory<Confluent.Kafka.Message>> _commandFactories;
        private readonly ITracer _tracer;

        private readonly IAccessorTypesProvider _accessorTypesProvider = new AccessorTypesProvider();
        private readonly DefaultKafkaBatchSizeSettings _batchSizeSettings = new DefaultKafkaBatchSizeSettings();

        public KafkaReplicationActor(
            IConnectionStringSettings connectionStringSettings,
            IDataObjectTypesProviderFactory dataObjectTypesProviderFactory,
            IKafkaMessageFlowReceiverFactory kafkaMessageFlowReceiverFactory,
            KafkaMessageFlowInfoProvider kafkaMessageFlowInfoProvider,
            IReadOnlyCollection<IBulkCommandFactory<Confluent.Kafka.Message>> commandFactories,
            ITracer tracer)
        {
            _connectionStringSettings = connectionStringSettings;
            _dataObjectTypesProviderFactory = dataObjectTypesProviderFactory;
            _receiverFactory = kafkaMessageFlowReceiverFactory;
            _kafkaMessageFlowInfoProvider = kafkaMessageFlowInfoProvider;
            _commandFactories = commandFactories;
            _tracer = tracer;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            foreach (var kafkaCommand in commands.OfType<KafkaReplicationCommand>())
            {
                var command = kafkaCommand.ReplicateInBulkCommand;
                var dataObjectTypes = GetDataObjectTypes(command);

                using (var transation = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionOptions))
                using (var targetConnection = CreateDataConnection(command.TargetStorageDescriptor))
                {
                    var resolvedCommandFactories = _commandFactories.Where(f => f.AppropriateFlows.Contains(kafkaCommand.MessageFlow))
                                                                    .ToList();
                    var actors = CreateActors(dataObjectTypes,
                                              targetConnection,
                                              new BulkCopyOptions
                                              {
                                                  BulkCopyTimeout = (int)command.BulkCopyTimeout.TotalSeconds
                                              });

                    var targetMessageFlowDescription = kafkaCommand.MessageFlow.GetType().Name;
                    using (var receiver = _receiverFactory.Create(kafkaCommand.MessageFlow))
                    {
                        // retry добавлен из-за https://github.com/confluentinc/confluent-kafka-dotnet/issues/86
                        var lastTargetMessageOffset =
                            Policy.Handle<Confluent.Kafka.KafkaException>(exception => exception.Error.Code == Confluent.Kafka.ErrorCode.LeaderNotAvailable)
                                  .WaitAndRetryForever(i => TimeSpan.FromSeconds(5),
                                                       (exception, waitSpan) =>
                                                           _tracer.Warn(exception,
                                                                        $"Can't get size of kafka topic. Message flow: {targetMessageFlowDescription}. Wait span: {waitSpan}"))
                                  .ExecuteAndCapture(() => _kafkaMessageFlowInfoProvider.GetFlowSize(kafkaCommand.MessageFlow) - 1)
                                  .Result;

                        _tracer.Info($"Receiving messages from kafka for flow: {targetMessageFlowDescription}. Last target message offset: {lastTargetMessageOffset}");

                        long currentMessageOffset = 0;
                        int receivedMessagesQuantity = 0;
                        while (currentMessageOffset < lastTargetMessageOffset)
                        {
                            var batch = receiver.ReceiveBatch(_batchSizeSettings.BatchSize);
                            // крутим цикл пока не получим сообщения от kafka,
                            // т.к. у приемника kafka есть некоторое время прогрева, то после запуска некоторое время могут возвращаться пустые batch,
                            // несмотря на фактическое наличие сообщений
                            if (batch.Count == 0)
                            {
                                continue;
                            }

                            receivedMessagesQuantity += batch.Count;
                            currentMessageOffset = batch.Last().Offset.Value;

                            _tracer.Info($"Flow: {targetMessageFlowDescription}. Received messages: {batch.Count}. Last message offset for received batch: {currentMessageOffset}. Target and current offsets distance: {lastTargetMessageOffset - currentMessageOffset}");

                            var bulkCommands = resolvedCommandFactories.SelectMany(factory => factory.CreateCommands(batch))
                                                                       .ToList();

                            _tracer.Info($"Flow: {targetMessageFlowDescription}. Actors count: {actors.Count}. Bulk commands count: {bulkCommands.Count}");

                            if (bulkCommands.Count > 0)
                            {
                                foreach (var actor in actors)
                                {
                                    actor.ExecuteCommands(bulkCommands);
                                }
                            }

                            receiver.CompleteBatch(batch);
                        }

                        _tracer.Info($"Receiving messages from kafka for flow: {targetMessageFlowDescription} finished. Received messages quantity: {receivedMessagesQuantity}");
                    }

                    transation.Complete();
                }
            }

            return Array.Empty<IEvent>();
        }

        private IReadOnlyCollection<Type> GetDataObjectTypes(ReplicateInBulkCommand command)
        {
            var dataObjectTypesProvider = (DataObjectTypesProvider)_dataObjectTypesProviderFactory.Create(command);
            return dataObjectTypesProvider.DataObjectTypes;
        }

        private IReadOnlyCollection<IActor> CreateActors(IReadOnlyCollection<Type> dataObjectTypes, DataConnection dataConnection, BulkCopyOptions bulkCopyOptions)
        {
            var actors = new List<IActor>();

            foreach (var dataObjectType in dataObjectTypes)
            {
                var accessorTypes = _accessorTypesProvider.GetAccessorsFor(dataObjectType);
                foreach (var accessorType in accessorTypes)
                {
                    var accessor = Activator.CreateInstance(accessorType, (IQuery)null);
                    var actorType = typeof(BulkInsertDataObjectsActor<>).MakeGenericType(dataObjectType);
                    var actor = (IActor)Activator.CreateInstance(actorType, accessor, dataConnection, bulkCopyOptions);

                    actors.Add(actor);
                }
            }

            return actors;
        }

        #region copy-paste from StateInitialization.Core

        private static readonly TransactionOptions TransactionOptions =
            new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.Serializable,
                    Timeout = TimeSpan.Zero
                };

        private DataConnection CreateDataConnection(StorageDescriptor storageDescriptor)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(storageDescriptor.ConnectionStringIdentity);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptor.MappingSchema);
            connection.CommandTimeout = (int)storageDescriptor.CommandTimeout.TotalMilliseconds;
            return connection;
        }

        #endregion

        #region smart copy-paste from StateInitialization.Core

        private sealed class AccessorTypesProvider : IAccessorTypesProvider
        {
            private static readonly Lazy<IReadOnlyDictionary<Type, Type[]>> AccessorTypes = new Lazy<IReadOnlyDictionary<Type, Type[]>>(LoadAccessorTypes);

            private static IReadOnlyDictionary<Type, Type[]> LoadAccessorTypes()
                => AppDomain.CurrentDomain.GetAssemblies()
                            .Where(x => !x.IsDynamic)
                            .SelectMany(SafeGetAssemblyExportedTypes)
                            .SelectMany(type => type.GetInterfaces(), (type, @interface) => new { type, @interface })
                            .Where(x => !x.type.IsAbstract && x.@interface.IsGenericType && x.@interface.GetGenericTypeDefinition() == typeof(IMemoryBasedDataObjectAccessor<>))
                            .Select(x => new { GenericArgument = x.@interface.GetGenericArguments()[0], Type = x.type })
                            .GroupBy(x => x.GenericArgument, x => x.Type)
                            .ToDictionary(x => x.Key, x => x.ToArray());

            private static IEnumerable<Type> SafeGetAssemblyExportedTypes(Assembly assembly)
            {
                try
                {
                    return assembly.ExportedTypes;
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            }

            public IReadOnlyCollection<Type> GetAccessorsFor(Type dataObjectType) => AccessorTypes.Value
                                                                                                  .TryGetValue(dataObjectType, out Type[] result)
                                                                                         ? result
                                                                                         : Array.Empty<Type>();
        }

        // BulkInsertDataObjectsCommand для IMemoryBasedDataObjectAccessor
        internal sealed class BulkInsertDataObjectsCommand : ICommand
        {
            public BulkInsertDataObjectsCommand(Type dataObjectType, IEnumerable<object> dtos)
            {
                DataObjectType = dataObjectType;
                Dtos = dtos;
            }

            public Type DataObjectType { get; }
            public IEnumerable<object> Dtos { get; }
        }


        // BulkInsertDataObjectsActor для IMemoryBasedDataObjectAccessor
        private sealed class BulkInsertDataObjectsActor<TDataObject> : IActor
            where TDataObject : class
        {
            private static readonly Type DataObjectType = typeof(TDataObject);

            private readonly IMemoryBasedDataObjectAccessor<TDataObject> _dataObjectAccessor;
            private readonly BulkCopyOptions _bulkCopyOptions;
            private readonly ITable<TDataObject> _table;
            private readonly PropertyInfo _predicateInfo;

            public BulkInsertDataObjectsActor(IMemoryBasedDataObjectAccessor<TDataObject> dataObjectAccessor, DataConnection dataConnection, BulkCopyOptions bulkCopyOptions)
            {
                _dataObjectAccessor = dataObjectAccessor;
                _bulkCopyOptions = bulkCopyOptions;
                _table = dataConnection.GetTable<TDataObject>();

                // хак чтобы не городить dependency injection для IQuery, потом обсудить как сделать правильно
                _predicateInfo = typeof(FindSpecification<TDataObject>).GetProperty("Predicate", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
            {
                var command = commands.OfType<BulkInsertDataObjectsCommand>().SingleOrDefault(x => x.DataObjectType == DataObjectType);
                if (command != null)
                {
                    var replaceDataObjectCommand = new ReplaceDataObjectCommand(DataObjectType, command.Dtos);

                    var findSpecification = _dataObjectAccessor.GetFindSpecification(replaceDataObjectCommand);
                    var predicate = (Expression<Func<TDataObject, bool>>)_predicateInfo.GetValue(findSpecification);
                    _table.Delete(predicate);

                    var dataObjects = _dataObjectAccessor.GetDataObjects(replaceDataObjectCommand);
                    ExecuteBulkCopy(dataObjects);
                }

                return Array.Empty<IEvent>();
            }

            private void ExecuteBulkCopy(IEnumerable<TDataObject> source)
            {
                try
                {
                    _table.BulkCopy(_bulkCopyOptions, source);
                }
                catch (Exception ex)
                {
                    throw new DataException($"Error occured while bulk replacing data for dataobject of type {typeof(TDataObject).Name} using {_dataObjectAccessor.GetType().Name}{Environment.NewLine}", ex);
                }
            }
        }

        #endregion

        private sealed class DefaultKafkaBatchSizeSettings
        {
            private readonly IntSetting _batchSize = ConfigFileSetting.Int.Optional("DefaultKafkaBatchSize", 5000);

            public int BatchSize => _batchSize.Value;
        }

        // CommandRegardlessDataObjectTypesProvider - он internal в StateInitiallization.Core, пришлось запилить вот это
        internal sealed class DataObjectTypesProvider : IDataObjectTypesProvider
        {
            public IReadOnlyCollection<Type> DataObjectTypes { get; }

            public DataObjectTypesProvider(IReadOnlyCollection<Type> dataObjectTypes)
            {
                DataObjectTypes = dataObjectTypes;
            }

            public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand => throw new NotImplementedException();
        }
    }
}
