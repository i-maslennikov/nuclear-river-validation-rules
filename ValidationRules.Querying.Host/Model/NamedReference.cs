using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    public class NamedReference
    {
       public NamedReference(IEntityType type, long id, string name)
        {
            Type = type.Description;
            Id = id;
            Name = name;
        }

        public string Type { get; }
        public string Name { get; }
        public long Id { get; }
    }

    public class OrderPositionNamedReference : NamedReference
    {
        public OrderPositionNamedReference(long id, NamedReference packagePosition, NamedReference itemPosition, NamedReference order)
            : base(EntityTypeOrderPosition.Instance, id, packagePosition.Name)
        {
            Order = order;
            PackagePosition = packagePosition;
            ItemPosition = itemPosition;
        }

        public NamedReference Order { get; }

        public string PositionPrefix
            => PackagePosition.Id == ItemPosition.Id
                ? Resources.RichDefaultPositionTypeTemplate
                : string.Format(Resources.RichChildPositionTypeTemplate, ItemPosition.Name);

        private NamedReference PackagePosition { get; }
        private NamedReference ItemPosition { get; }
    }
}