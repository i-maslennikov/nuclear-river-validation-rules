using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAdvertisement : EntityTypeBase<EntityTypeAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.Advertisement;
        public override string Description { get; } = nameof(EntityTypeIds.Advertisement);
    }
}
