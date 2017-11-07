using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory<TrackedUseCase> _commandFactory;

        public FactsFlowAccumulator()
        {
            _commandFactory = new FactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(trackedUseCase),
            };
        }
    }
}