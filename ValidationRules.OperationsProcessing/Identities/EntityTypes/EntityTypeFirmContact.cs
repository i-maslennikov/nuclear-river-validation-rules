using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmContact : EntityTypeBase<EntityTypeFirmContact>
    {
        public override int Id { get; } = EntityTypeIds.FirmContact;
        public override string Description { get; } = nameof(EntityTypeIds.FirmContact);
    }
}