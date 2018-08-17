using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using Microsoft.SqlServer.Management.Smo;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.StateInitialization.Core.Storage;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;

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

        private readonly IAccessorTypesProvider _accessorTypesProvider = new InMemoryAccessorTypesProvider();
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

                using (var targetConnection = CreateDataConnection(command.TargetStorageDescriptor))
                {
                    LoadDataFromKafka2Db(kafkaCommand.MessageFlow,
                                         dataObjectTypes,
                                         targetConnection,
                                         (int)command.BulkCopyTimeout.TotalSeconds);

                    if (!command.DbManagementMode.HasFlag(DbManagementMode.UpdateTableStatistics))
                    {
                        continue;
                    }

                    IReadOnlyCollection<ICommand> updateStatisticsCommands =
                        dataObjectTypes.Select(t => command.TargetStorageDescriptor.MappingSchema.GetTableName(t))
                                       .Select(table => new UpdateTableStatisticsActor.UpdateTableStatisticsCommand(table,
                                                                                                                    StatisticsTarget.All,
                                                                                                                    StatisticsScanType.FullScan))
                                       .ToList();
                    var updateStatisticsActor = new UpdateTableStatisticsActor((SqlConnection)targetConnection.Connection);
                    updateStatisticsActor.ExecuteCommands(updateStatisticsCommands);
                }
            }

            return Array.Empty<IEvent>();
        }

        private void LoadDataFromKafka2Db(IMessageFlow messageFlowForKafkaTopic,
                                          IReadOnlyCollection<Type> dataObjectTypes,
                                          DataConnection dataConnection,
                                          int bulkReplaceCommandTimeoutSec)
        {
            var targetMessageFlowDescription = messageFlowForKafkaTopic.GetType().Name;

            var actors = CreateActors(dataObjectTypes,
                                      dataConnection,
                                      new BulkCopyOptions
                                          {
                                              BulkCopyTimeout = bulkReplaceCommandTimeoutSec
                                          });

            using (var receiver = _receiverFactory.Create(messageFlowForKafkaTopic))
            {
                // retry добавлен из-за https://github.com/confluentinc/confluent-kafka-dotnet/issues/86
                var lastTargetMessageOffset =
                    Policy.Handle<Confluent.Kafka.KafkaException>(exception => exception.Error.Code == Confluent.Kafka.ErrorCode.LeaderNotAvailable)
                          .WaitAndRetryForever(i => TimeSpan.FromSeconds(5),
                                               (exception, waitSpan) =>
                                                   _tracer.Warn(exception,
                                                                $"Can't get size of kafka topic. Message flow: {targetMessageFlowDescription}. Wait span: {waitSpan}"))
                          .ExecuteAndCapture(() => _kafkaMessageFlowInfoProvider.GetFlowSize(messageFlowForKafkaTopic) - 1)
                          .Result;

                _tracer.Info($"Receiving messages from kafka for flow: {targetMessageFlowDescription}. Last target message offset: {lastTargetMessageOffset}");

                var resolvedCommandFactories = _commandFactories.Where(f => f.AppropriateFlows.Contains(messageFlowForKafkaTopic))
                                                                .ToList();

                using (var transation = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                             new TransactionOptions
                                                                 {
                                                                     IsolationLevel = IsolationLevel.Serializable,
                                                                     Timeout = TimeSpan.Zero
                                                                 }))
                {
                    long currentMessageOffset = 0;
                    int receivedMessagesQuantity = 0;
                    while (currentMessageOffset < lastTargetMessageOffset)
                    {
                        var batch = receiver.ReceiveBatch(_batchSizeSettings.BatchSize);
                        // крутим цикл пока не получим сообщения от kafka,
                        // т.к. у клиента kafka есть некоторое время прогрева, то после запуска некоторое время могут возвращаться пустые batch,
                        // несмотря на фактическое наличие сообщений в topic\partition
                        if (batch.Count == 0)
                        {
                            continue;
                        }

                        receivedMessagesQuantity += batch.Count;
                        currentMessageOffset = batch.Last().Offset.Value;

                        _tracer.Info($"Flow: {targetMessageFlowDescription}. Received messages: {batch.Count}. Last message offset for received batch: {currentMessageOffset}. Target and current offsets distance: {lastTargetMessageOffset - currentMessageOffset}");

                        var bulkCommands = resolvedCommandFactories.SelectMany(factory => factory.CreateCommands(batch))
                                                                   .ToList();

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
                    transation.Complete();
                }
            }
        }

        private IReadOnlyCollection<Type> GetDataObjectTypes(ReplicateInBulkCommand command)
        {
            var dataObjectTypesProvider = (DataObjectTypesProviderFactory.DataObjectTypesProvider)_dataObjectTypesProviderFactory.Create(command);
            return dataObjectTypesProvider.DataObjectTypes;
        }

        private IReadOnlyCollection<IActor> CreateActors(IReadOnlyCollection<Type> dataObjectTypes,
                                                         DataConnection dataConnection,
                                                         BulkCopyOptions bulkCopyOptions)
        {
            var actors = new List<IActor>();

            foreach (var dataObjectType in dataObjectTypes)
            {
                var accessorTypes = _accessorTypesProvider.GetAccessorsFor(dataObjectType);
                foreach (var accessorType in accessorTypes)
                {
                    var accessor = Activator.CreateInstance(accessorType, (IQuery)null);
                    var actorType = typeof(BulkInsertInMemoryDataObjectsActor<>).MakeGenericType(dataObjectType);
                    var actor = (IActor)Activator.CreateInstance(actorType, accessor, dataConnection, bulkCopyOptions);

                    actors.Add(actor);
                }
            }

            return actors;
        }

        private DataConnection CreateDataConnection(StorageDescriptor storageDescriptor)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(storageDescriptor.ConnectionStringIdentity);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptor.MappingSchema);
            connection.CommandTimeout = (int)storageDescriptor.CommandTimeout.TotalMilliseconds;
            return connection;
        }

        private sealed class DefaultKafkaBatchSizeSettings
        {
            private readonly IntSetting _batchSize = ConfigFileSetting.Int.Optional("DefaultKafkaBatchSize", 5000);

            public int BatchSize => _batchSize.Value;
        }
    }
}
