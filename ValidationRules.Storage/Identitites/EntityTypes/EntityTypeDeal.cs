using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeDeal : EntityTypeBase<EntityTypeDeal>
    {
        public override int Id => EntityTypeIds.Deal;

        public override string Description => nameof(EntityTypeIds.Deal);
    }
}