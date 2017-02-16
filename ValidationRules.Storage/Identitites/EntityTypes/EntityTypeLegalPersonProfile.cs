using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeLegalPersonProfile : EntityTypeBase<EntityTypeLegalPersonProfile>
    {
        public override int Id { get; } = EntityTypeIds.LegalPersonProfile;
        public override string Description { get; } = nameof(EntityTypeIds.LegalPersonProfile);
    }
}