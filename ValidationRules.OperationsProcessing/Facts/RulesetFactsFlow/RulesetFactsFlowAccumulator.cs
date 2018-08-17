using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;

using ValidationRules.Hosting.Common.Settings;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.RulesetFactsFlow
{
    public sealed class RulesetFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<RulesetFactsFlow, KafkaMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory<KafkaMessage> _commandFactory;

        public RulesetFactsFlowAccumulator(IBusinessModelSettings businessModelSettings)
        {
            _commandFactory = new RulesetFactsCommandFactory(businessModelSettings);
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