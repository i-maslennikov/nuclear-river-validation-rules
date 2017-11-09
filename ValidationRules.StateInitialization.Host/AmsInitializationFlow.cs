using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;

using NuClear.Messaging.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    internal sealed class AmsInitializationFlow : IEnumerable<ICommand>
    {
        private readonly int _batchSize;
        private readonly int _minBatchSize;
        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;
        private readonly AmsFactsCommandFactory _commandFactory;

        public AmsInitializationFlow(IConnectionStringSettings connectionStringSettings)
        {
            var amsSettingsFactory = new AmsSettingsFactory(connectionStringSettings, new EnvironmentSettingsAspect(), Offset.Beginning);
            _receiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), amsSettingsFactory);
            _commandFactory = new AmsFactsCommandFactory();
            _batchSize = ConfigFileSetting.Int.Optional("AmsBatchSize", 50000).Value;
            _minBatchSize = 1000;
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            using (var receiver = _receiverFactory.Create(AmsFactsFlow.Instance))
            {
                while(true)
                {
                    var batch = receiver.ReceiveBatch(_batchSize);
                    var maxOffsetMesasage = batch.OrderByDescending(x => x.Offset.Value).First();
                    Console.WriteLine($"Received {batch.Count} messages, offset {maxOffsetMesasage.Offset}");

                    var commands = batch
                        .Where(x => x.Value != null)
                        .SelectMany(_commandFactory.CreateCommands)
                        .OfType<IReplaceDataObjectCommand>()
                        .GroupBy(x => x.DataObjectType);

                    foreach (var commandGroup in commands)
                    {
                        if (commandGroup.Key == typeof(Advertisement))
                        {
                            yield return new ReplaceDataObjectCommand<Advertisement>(typeof(Advertisement),
                                commandGroup.Cast<ReplaceDataObjectCommand<Advertisement>>().SelectMany(x => x.DataObjects).ToList());
                        }

                        if (commandGroup.Key == typeof(EntityName))
                        {
                            yield return new ReplaceDataObjectCommand<EntityName>(typeof(EntityName),
                                commandGroup.Cast<ReplaceDataObjectCommand<EntityName>>().SelectMany(x => x.DataObjects).ToList());
                        }
                    }

                    receiver.CompleteBatch(batch);

                    // state init имеет смысл прекращать когда мы вычитали все полные батчи
                    // а то нам могут до бесконечности подкидывать новых messages
                    if (batch.Count < _minBatchSize)
                    {
                        break;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
