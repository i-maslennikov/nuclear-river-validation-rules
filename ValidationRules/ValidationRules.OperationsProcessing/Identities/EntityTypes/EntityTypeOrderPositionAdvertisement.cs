using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeOrderPositionAdvertisement : EntityTypeBase<EntityTypeOrderPositionAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.OrderPositionAdvertisement;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPositionAdvertisement);
    }
}
