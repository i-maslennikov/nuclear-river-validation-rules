using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeBargainFile : EntityTypeBase<EntityTypeBargainFile>
    {
        public override int Id { get; } = EntityTypeIds.BargainFile;
        public override string Description { get; } = nameof(EntityTypeIds.BargainFile);
    }
}