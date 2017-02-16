using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeBargain : EntityTypeBase<EntityTypeBargain>
    {
        public override int Id { get; } = EntityTypeIds.Bargain;
        public override string Description { get; } = nameof(EntityTypeIds.Bargain);
    }
}