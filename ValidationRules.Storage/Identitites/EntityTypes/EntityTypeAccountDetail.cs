using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAccountDetail : EntityTypeBase<EntityTypeAccountDetail>
    {
        public override int Id { get; } = EntityTypeIds.AccountDetail;
        public override string Description { get; } = nameof(EntityTypeIds.AccountDetail);
    }
}