using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeDeal : EntityTypeBase<EntityTypeDeal>
    {
        public override int Id => EntityTypeIds.Deal;

        public override string Description => nameof(EntityTypeIds.Deal);
    }
}