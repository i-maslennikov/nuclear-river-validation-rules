using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAdvertisementElementTemplate : EntityTypeBase<EntityTypeAdvertisementElementTemplate>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementElementTemplate;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementElementTemplate);
    }
}
