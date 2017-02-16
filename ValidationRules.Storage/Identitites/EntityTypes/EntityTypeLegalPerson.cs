using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeLegalPerson : EntityTypeBase<EntityTypeLegalPerson>
    {
        public override int Id => EntityTypeIds.LegalPerson;

        public override string Description => nameof(EntityTypeIds.LegalPerson);
    }
}