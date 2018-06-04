using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Hosting.Common.Settings;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<RulesetFactsFlow, KafkaMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory<KafkaMessage> _commandFactory;

        public RulesetFactsFlowAccumulator(IEnvironmentSettings environmentSettings)
        {
            _commandFactory = new RulesetFactsCommandFactory(environmentSettings);
        }

        protected override AggregatableMessage<ICommand> Process(KafkaMessage kafkaMessage)
        {
            return new AggregatableMessage<ICommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = _commandFactory.CreateCommands(kafkaMessage),
                };
        }
    }
}