using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAdvertisementElement : EntityTypeBase<EntityTypeAdvertisementElement>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementElement;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementElement);
    }
}
