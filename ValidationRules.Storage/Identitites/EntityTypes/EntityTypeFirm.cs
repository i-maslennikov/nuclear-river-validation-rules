using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id { get; } = EntityTypeIds.Firm;
        public override string Description { get; } = nameof(EntityTypeIds.Firm);
    }
}