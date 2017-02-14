using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeBranchOffice : EntityTypeBase<EntityTypeBranchOffice>
    {
        public override int Id => EntityTypeIds.BranchOffice;

        public override string Description => nameof(EntityTypeIds.BranchOffice);
    }
}