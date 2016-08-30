using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeOrganizationUnit : EntityTypeBase<EntityTypeOrganizationUnit>
    {
        public override int Id { get; } = EntityTypeIds.OrganizationUnit;
        public override string Description { get; } = nameof(EntityTypeIds.OrganizationUnit);
    }
}
