using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeCategoryFirmAddress : EntityTypeBase<EntityTypeCategoryFirmAddress>
    {
        public override int Id { get; } = EntityTypeIds.CategoryFirmAddress;
        public override string Description { get; } = nameof(EntityTypeIds.CategoryFirmAddress);
    }
}