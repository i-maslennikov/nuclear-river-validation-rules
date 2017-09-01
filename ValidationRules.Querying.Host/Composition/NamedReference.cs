using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public class NamedReference
    {
        public NamedReference(Reference reference, string name)
        {
            Reference = reference;
            Name = name;
        }

        public Reference Reference { get; set; }
        public string Name { get; }
    }

    public class OrderPositionNamedReference : NamedReference
    {
        public OrderPositionNamedReference(Reference orderPosition, NamedReference packagePosition, NamedReference itemPosition, NamedReference order)
            : base(orderPosition, packagePosition.Name)
        {
            Order = order;
            PackagePosition = packagePosition;
            ItemPosition = itemPosition;
        }

        public NamedReference Order { get; }

        public string PositionPrefix
            => PackagePosition.Reference.Id == ItemPosition.Reference.Id
                ? Resources.RichDefaultPositionTypeTemplate
                : string.Format(Resources.RichChildPositionTypeTemplate, ItemPosition.Name.ClearBrackets());

        private NamedReference PackagePosition { get; }
        private NamedReference ItemPosition { get; }
    }
}