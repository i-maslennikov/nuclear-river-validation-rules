using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeOrderFile : EntityTypeBase<EntityTypeOrderFile>
    {
        public override int Id { get; } = EntityTypeIds.OrderFile;
        public override string Description { get; } = nameof(EntityTypeIds.OrderFile);
    }
}