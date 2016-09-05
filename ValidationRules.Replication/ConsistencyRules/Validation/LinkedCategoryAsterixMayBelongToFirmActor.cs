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
    /// Для заказов, к которым привязана рубрика, не принадлежащая фирме, если объект привязки является "рубрика множественная со звёздочкой", должно выводиться информационное сообщение.
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая фирме заказа"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryAsterixMayBelongToFirmActor : IActor
    {
        public const int MessageTypeId = 32;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Info)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Info)
                                                                    .WhenMassRelease(Result.Info);

        private readonly ValidationRuleShared _validationRuleShared;

        public LinkedCategoryAsterixMayBelongToFirmActor(ValidationRuleShared validationRuleShared)
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
                              from category in query.For<Order.InvalidCategory>().Where(x => x.OrderId == order.Id)
                              where category.State == InvalidCategoryState.NotBelongToFirm && category.MayNotBelongToFirm
                              select new Version.ValidationResult
                              {
                                  MessageType = MessageTypeId,
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("category",
                                                                    new XAttribute("id", category.CategoryId),
                                                                    new XAttribute("name", category.CategoryName)),
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
