using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;

namespace NuClear.ValidationRules.OperationsProcessing.Events
{
    internal sealed class ImportFactsFromErmEvent : IEvent
    {
        public TrackedUseCase TrackedUseCase { get; }

        public ImportFactsFromErmEvent(TrackedUseCase trackedUseCase)
        {
            TrackedUseCase = trackedUseCase;
        }
    }
}
