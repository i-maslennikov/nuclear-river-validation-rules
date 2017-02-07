using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeCategoryOrganizationUnit : EntityTypeBase<EntityTypeCategoryOrganizationUnit>
    {
        public override int Id { get; } = EntityTypeIds.CategoryOrganizationUnit;
        public override string Description { get; } = nameof(EntityTypeIds.CategoryOrganizationUnit);
    }
}