using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;

using NuClear.ValidationRules.Storage.Model.Aggregates;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors.Validation
{
    public sealed class OrderPositionCorrespontToInactivePositionActor : IActor
    {
        // OrderCheckOrderPositionCorrespontToInactivePosition - Позиция {OrderPositionId} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа.
        private const int MessageTypeId = 4;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public OrderPositionCorrespontToInactivePositionActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            // проверка проверяет соответствие только первого периода
            var orderFirstPeriods = from orderPeriod1 in query.For<OrderPeriod>()
                                    from orderPeriod2 in query.For<OrderPeriod>().Where(x => orderPeriod1.OrderId == x.OrderId && orderPeriod1.Start > x.Start).DefaultIfEmpty()
                                    where orderPeriod2 == null
                                    select orderPeriod1;

            var orderFirstPeriodDtos = from orderFirstPeriod in orderFirstPeriods
                                  join order in query.For<Order>() on orderFirstPeriod.OrderId equals order.Id
                                  join period in query.For<Period>()
                                  on new { orderFirstPeriod.OrganizationUnitId, orderFirstPeriod.Start } equals new { period.OrganizationUnitId, period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,

                                      period.ProjectId,
                                      period.Start,
                                      period.End,
                                  };

            var pricePositionIsNotActiveErrors =
            from orderFirstPeriodDto in orderFirstPeriodDtos
            join orderPricePosition in query.For<OrderPricePosition>() on orderFirstPeriodDto.OrderId equals orderPricePosition.OrderId
            where !orderPricePosition.IsActive
            select new Version.ValidationResult
            {
                MessageType = MessageTypeId,
                MessageParams = new XDocument(new XElement("empty",
                                                new XAttribute("id", orderPricePosition.OrderPositionId),
                                                new XAttribute("name", orderPricePosition.OrderPositionName)
                                             )),

                PeriodStart = orderFirstPeriodDto.Start,
                PeriodEnd = orderFirstPeriodDto.End,
                ProjectId = orderFirstPeriodDto.ProjectId,

                VersionId = version,

                ReferenceType = EntityTypeIds.Order,
                ReferenceId = orderFirstPeriodDto.OrderId,

                Result = RuleResult,
            };

            return pricePositionIsNotActiveErrors;
        }
    }
}