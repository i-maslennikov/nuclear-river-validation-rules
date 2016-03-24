using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Identities.Operations
{
    public sealed class RecalculateAggregatePartOperationIdentity : OperationIdentityBase<RecalculateAggregatePartOperationIdentity>
    {
        public override int Id { get { return 0; } }

        public Guid Guid
        {
            get { return Guid.Parse("A22420E7-B7E7-4C7A-BF39-41D53C9C74EF"); }
        }

        public override string Description
        {
            get { return "Операция пересчёта части агрегата"; }
        }
    }

}