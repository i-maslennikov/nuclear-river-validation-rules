using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLegalPerson : EntityTypeBase<EntityTypeLegalPerson>
    {
        public override int Id => EntityTypeIds.LegalPerson;

        public override string Description => nameof(EntityTypeIds.LegalPerson);
    }
}