using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmAddress : EntityTypeBase<EntityTypeFirmAddress>
    {
        public override int Id { get; } = EntityTypeIds.FirmAddress;
        public override string Description { get; } = nameof(EntityTypeIds.FirmAddress);
    }
}