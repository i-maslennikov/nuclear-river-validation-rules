using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id
        {
            get { return EntityTypeIds.Order; }
        }

        public override string Description
        {
            get { return "Order"; }
        }
    }
}