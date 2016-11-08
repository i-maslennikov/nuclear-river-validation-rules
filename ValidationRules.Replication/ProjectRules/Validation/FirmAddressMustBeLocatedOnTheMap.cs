using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, с привязками к не отмеченным на карте адресам, должна выводиться ошибка.
    /// "Позиция {0} оформлена на пустой адрес {1}"
    /// -> В позиции {0} найден адрес {1}, не привязанный к карте
    /// 
    /// Source: IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule
    /// * Проверка очень похожа на LinkedFirmAddressShouldBeValid, но запускается в ручном массовом. А в целом очень хочется их объединить.
    /// </summary>
    public sealed class FirmAddressMustBeLocatedOnTheMap : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmAddressMustBeLocatedOnTheMap(IQuery query) : base(query, MessageTypeCode.FirmAddressMustBeLocatedOnTheMap)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from advertisement in query.For<Order.AddressAdvertisement>().Where(x => x.OrderId == order.Id && x.MustBeLocatedOnTheMap)
                from firmAddress in query.For<FirmAddress>().Where(x => x.Id == advertisement.AddressId)
                from position in query.For<Position>().Where(x => x.Id == advertisement.PositionId)
                where !firmAddress.IsLocatedOnTheMap
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("firmAddress",
                                    new XAttribute("id", firmAddress.Id),
                                    new XAttribute("name", firmAddress.Name)),
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)),
                                new XElement("orderPosition",
                                    new XAttribute("id", advertisement.OrderPositionId),
                                    new XAttribute("name", position.Name)))),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}