using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private const int TotalWaitMilliseconds = 60000;

        private readonly IQuery _query;
        private readonly ITracer _tracer;
        private readonly ICommandFactory<TrackedUseCase> _commandFactory;

        public FactsFlowAccumulator(IQuery query, ITracer tracer)
        {
            _query = query;
            _tracer = tracer;
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