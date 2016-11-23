using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBranchOffice : EntityTypeBase<EntityTypeBranchOffice>
    {
        public override int Id => EntityTypeIds.BranchOffice;

        public override string Description => nameof(EntityTypeIds.BranchOffice);
    }
}