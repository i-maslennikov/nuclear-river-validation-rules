using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Dto
{
    public sealed class OrderValidationConfigParser : IConfigParser
    {
        public IDataTransferObject Parse(Stream config)
        {
            var xml = XElement.Load(config);
            var positions = xml.XPathSelectElements(@"associatedDeniedPositions/position");
            return new OrderValidationConfig { Positions = ParsePositions(positions).ToArray() };
        }

        private IEnumerable<OrderValidationConfig.Position> ParsePositions(IEnumerable<XElement> positions)
        {
            return positions.GroupBy(x => (long)x.Attribute("id")).Select(group => new OrderValidationConfig.Position
                {
                    Id = group.Key,
                    MasterPositions = group.SelectMany(x => x.XPathSelectElements(@"masterPositions/position")).Select(ParsePositionBinding).ToArray(),
                    DeniedPositions = group.SelectMany(x => x.XPathSelectElements(@"deniedPositions/position")).Select(ParsePositionBinding).ToArray(),
                });
        }

        private OrderValidationConfig.PositionBinding ParsePositionBinding(XElement element)
        {
            return new OrderValidationConfig.PositionBinding
                {
                    Id = (long)element.Attribute("id"),
                    BindingType = (string)element.Attribute("bindingType"),
                };
        }
    }
}
