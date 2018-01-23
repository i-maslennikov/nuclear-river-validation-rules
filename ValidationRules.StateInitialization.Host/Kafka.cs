using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;

using Confluent.Kafka;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using Newtonsoft.Json;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.River.Hosting.Common.Settings;
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
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    internal sealed class KafkaReplicationCommand : ICommand
    {
        public KafkaReplicationCommand(IMessageFlow messageFlow, ReplicateInBulkCommand replicateInBulkCommand)
        {
            MessageFlow = messageFlow;
            ReplicateInBulkCommand = replicateInBulkCommand;
        }

        public IMessageFlow MessageFlow { get; }
        public ReplicateInBulkCommand ReplicateInBulkCommand { get; }
    }

    internal sealed class KafkaReplicationActor : IActor
    {
        // сколько неполных батчей мы запроцессим перед тем как выйти
        private const int NonMaxBatchCounter = 5;

        private readonly IDataObjectTypesProviderFactory _dataObjectTypesProviderFactory;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IAccessorTypesProvider _accessorTypesProvider;
        private readonly AmsBatchSizeSettings _batchSizeSettings;
        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;

        public KafkaReplicationActor(
            IDataObjectTypesProviderFactory dataObjectTypesProviderFactory,
            IConnectionStringSettings connectionStringSettings)
        {
            _dataObjectTypesProviderFactory = dataObjectTypesProviderFactory;
            _connectionStringSettings = connectionStringSettings;
            _batchSizeSettings = new AmsBatchSizeSettings();
            _accessorTypesProvider = new AccessorTypesProvider();

            var amsSettingsFactory = new KafkaSettingsFactory(connectionStringSettings, new EnvironmentSettingsAspect(), Offset.Beginning);
            _receiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), amsSettingsFactory);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            foreach (var kafkaCommand in commands.OfType<KafkaReplicationCommand>())
            {
                var command = kafkaCommand.ReplicateInBulkCommand;
                var dataObjectTypes = GetDataObjectTypes(command);

                ExecuteInTransactionScope(command,
                dataConnection =>
                {
                    var bulkCopyOptions = new BulkCopyOptions { BulkCopyTimeout = (int)command.BulkCopyTimeout.TotalSeconds };
                    var actors = CreateActors(dataObjectTypes, dataConnection, bulkCopyOptions);

                    using (var receiver = _receiverFactory.Create(kafkaCommand.MessageFlow))
                    {
                        var nonMaxBatchCounter = 0;

                        for(;;)
                        {
                            var batch = receiver.ReceiveBatch(_batchSizeSettings.BatchSize);

                            // крутим цикл пока не получим сообщения от kafka
                            if (batch.Count == 0)
                            {
                                continue;
                            }

                            var maxOffsetMesasage = batch.OrderByDescending(x => x.Offset.Value).First();
                            Console.WriteLine($"Received {batch.Count} messages, offset {maxOffsetMesasage.Offset}");

                            // filter heartbeat messages
                            var dtos = batch
                                .Where(x => x.Value != null)
                                .Select(x =>
                                {
                                    var dto = JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(x.Value));
                                    dto.Offset = x.Offset;
                                    return dto;
                                }).ToList();

                            if (dtos.Count != 0)
                            {
                                var bulkInsertCommands = new List<ICommand>
                                    {
                                        new BulkInsertDataObjectsCommand(typeof(Advertisement), dtos),
                                        new BulkInsertDataObjectsCommand(typeof(EntityName), dtos)
                                    };

                                foreach (var actor in actors)
                                {
                                    actor.ExecuteCommands(bulkInsertCommands);
                                }
                            }

                            receiver.CompleteBatch(batch);

                            // state init имеет смысл прекращать когда мы вычитали все полные батчи
                            // а то нам могут до бесконечности подкидывать новых messages
                            // 1 неполный batch ещё ничего не значит, это может быть начальный batch когда kafka разогревается
                            // выходим из цикла если вычитали NonMaxBatchCounter неполных батчей
                            if (batch.Count != _batchSizeSettings.BatchSize)
                            {
                                nonMaxBatchCounter++;
                            }
                            if (nonMaxBatchCounter == NonMaxBatchCounter)
                            {
                                break;
                            }
                        }
                    }
                });
            }

            return Array.Empty<IEvent>();

            IReadOnlyCollection<Type> GetDataObjectTypes(ReplicateInBulkCommand command)
            {
                var dataObjectTypesProvider = (DataObjectTypesProvider)_dataObjectTypesProviderFactory.Create(command);
                return dataObjectTypesProvider.DataObjectTypes;
            }
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

        private void ExecuteInTransactionScope(ReplicateInBulkCommand command, Action<DataConnection> action)
        {
            using (var transation = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionOptions))
            {
                using (var targetConnection = CreateDataConnection(command.TargetStorageDescriptor))
                {
                    action(targetConnection);
                    transation.Complete();
                }
            }
        }

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

            public IReadOnlyCollection<Type> GetAccessorsFor(Type dataObjectType) => AccessorTypes.Value.TryGetValue(dataObjectType, out Type[] result) ? result : Array.Empty<Type>();
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

        private sealed class AmsBatchSizeSettings
        {
            private readonly IntSetting _batchSize = ConfigFileSetting.Int.Optional("AmsBatchSize", 5000);

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
