using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAccount : EntityTypeBase<EntityTypeAccount>
    {
        public override int Id { get; } = EntityTypeIds.Account;
        public override string Description { get; } = nameof(EntityTypeIds.Account);
    }
}