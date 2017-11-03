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

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    internal sealed class AmsInitializationFlow : IEnumerable<ICommand>
    {
        private readonly int _batchSize;
        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;
        private readonly AmsFactsCommandFactory _commandFactory;

        public AmsInitializationFlow(IConnectionStringSettings connectionStringSettings)
        {
            var amsSettingsFactory = new AmsSettingsFactory(connectionStringSettings, new EnvironmentSettingsAspect(), Offset.Beginning);
            _receiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), amsSettingsFactory);
            _commandFactory = new AmsFactsCommandFactory();
            _batchSize = ConfigFileSetting.Int.Optional("AmsBatchSize", 5000).Value;
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            using (var receiver = _receiverFactory.Create(AmsFactsFlow.Instance))
            {
                // state init имеет смысл прекращать когда мы вычитали все полные батчи
                // а то нам могут до бесконечности подкидывать новых messages
                IReadOnlyCollection<Message> batch;
                while ((batch = receiver.ReceiveBatch(_batchSize)).Count == _batchSize)
                {
                    var maxOffsetMesasage = batch.OrderByDescending(x => x.Offset.Value).First();
                    Console.WriteLine($"Received {batch.Count} messages, offset {maxOffsetMesasage.Offset}");

                    var commands = batch
                        .Where(x => x.Value != null)
                        .SelectMany(_commandFactory.CreateCommands)
                        .OfType<IReplaceDataObjectCommand>();

                    // todo: рекомендую сделать группировку

                    foreach (var command in commands)
                    {
                        yield return command;
                    }

                    receiver.CompleteBatch(batch);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
