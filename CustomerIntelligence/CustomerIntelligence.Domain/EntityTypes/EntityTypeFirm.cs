using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id
        {
            get { return EntityTypeIds.Firm; }
        }

        public override string Description
        {
            get { return "Firm"; }
        }
    }
}