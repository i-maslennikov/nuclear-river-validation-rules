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
        public OrderPositionNamedReference(long id, string positionName, string itemPositionName, NamedReference order)
            : base(EntityTypeOrderPosition.Instance, id, positionName)
        {
            Order = order;
            PositionPrefix = string.Equals(itemPositionName, positionName)
                ? Resources.RichDefaultPositionTypeTemplate
                : string.Format(Resources.RichChildPositionTypeTemplate, itemPositionName);
        }

        public NamedReference Order { get; }
        public string PositionPrefix { get; }
    }
}