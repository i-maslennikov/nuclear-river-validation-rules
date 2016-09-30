using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAdvertisementTemplate : EntityTypeBase<EntityTypeAdvertisementTemplate>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementTemplate;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementTemplate);
    }
}
