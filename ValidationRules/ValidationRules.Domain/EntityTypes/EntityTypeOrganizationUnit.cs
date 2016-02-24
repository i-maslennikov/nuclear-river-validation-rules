using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeOrganizationUnit : EntityTypeBase<EntityTypeOrganizationUnit>
    {
        public override int Id { get; } = EntityTypeIds.OrganizationUnit;
        public override string Description { get; } = nameof(EntityTypeIds.OrganizationUnit);
    }
}
