using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов с продажами в рубрику адреса, к которым привязана рубрика не принадлежащая адресу, должно выводиться предупреждение в единичной, ошибка в массовой.
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryFirmAddressShouldBeValidActor : IActor
    {
        public const int MessageTypeId = 30;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public LinkedCategoryFirmAddressShouldBeValidActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var ruleResults = from order in query.For<Order>()
                              from categoryFirmAddress in query.For<Order.InvalidCategoryFirmAddress>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageType = MessageTypeId,
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("state", categoryFirmAddress.State)),
                                                       new XElement("firmAddress",
                                                                    new XAttribute("id", categoryFirmAddress.FirmAddressId),
                                                                    new XAttribute("name", categoryFirmAddress.FirmAddressName)),
                                                       new XElement("category",
                                                                    new XAttribute("id", categoryFirmAddress.CategoryId),
                                                                    new XAttribute("name", categoryFirmAddress.CategoryName)),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                                  PeriodStart = order.BeginDistribution,
                                  PeriodEnd = order.EndDistributionPlan,
                                  ProjectId = order.ProjectId,
                                  VersionId = version,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
