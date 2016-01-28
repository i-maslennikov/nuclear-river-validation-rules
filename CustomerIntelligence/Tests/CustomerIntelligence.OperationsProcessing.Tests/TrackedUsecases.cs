using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.OperationsTracking.API.Changes;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests
{
    public static class TrackedUsecases
    {
        public static TrackedUseCase UpdateFirms()
        {
            var entityType = new UnknownEntityType(1);
            var operationIdentity = new StrictOperationIdentity(UpdateIdentity.Instance, new EntitySet(entityType));
            var changes = new[]
                          {
                              ChangeDescriptor.Create(entityType, 12, ChangeKind.Updated),
                              ChangeDescriptor.Create(entityType, 13, ChangeKind.Updated)
                          };

            var operations = new[]
                             {
                                 new OperationDescriptor(Guid.NewGuid(), operationIdentity, new OperationContext(DateTimeOffset.UtcNow, DateTime.UtcNow), new EntityChangesContext(changes))
                             };

            var context = new UseCaseContext(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, 0);
            var useCase = new TrackedUseCase(context, operations[0].Id, operations, new Dictionary<Guid, HashSet<Guid>>());

            return useCase;
        }
    }
}