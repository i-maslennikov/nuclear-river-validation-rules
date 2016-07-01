using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    public sealed class ConflictingPrincipalPositionActor : IActor
    {
        private const int Different = 3;

        // ConflictingPrincipalPosition - {0} содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {1}
        private const int MessageTypeId = 11;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public ConflictingPrincipalPositionActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderAssociatedPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var satisfiedPositions =
                from orderPosition in orderPositions
                join associatedPosition in associatedPositions on
                    new { orderPosition.FirmId, orderPosition.Start, orderPosition.OrganizationUnitId, orderPosition.position.ItemPositionId } equals
                    new { associatedPosition.FirmId, associatedPosition.Start, associatedPosition.OrganizationUnitId, ItemPositionId = associatedPosition.position.PrincipalPositionId }

                where associatedPosition.position.BindingType != Different ||
                      (associatedPosition.position.HasNoBinding != orderPosition.position.HasNoBinding) ||
                      (associatedPosition.position.Category3Id != null && associatedPosition.position.Category3Id != orderPosition.position.Category3Id) ||
                      (associatedPosition.position.Category1Id != null && associatedPosition.position.Category1Id != orderPosition.position.Category1Id) ||
                      (associatedPosition.position.FirmAddressId != null && associatedPosition.position.FirmAddressId != orderPosition.position.FirmAddressId)
                select new
                {
                    associatedPosition.position.CauseOrderPositionId,
                    associatedPosition.position.CauseItemPositionId,
                };

            var notSatisfiedPositions =
                (from position in associatedPositions
                from satisfied in satisfiedPositions.Where(x => x.CauseOrderPositionId == position.position.CauseOrderPositionId &&
                                                                x.CauseItemPositionId == position.position.CauseItemPositionId).DefaultIfEmpty()
                where satisfied == null
                select new
                {
                    position.FirmId,
                    position.position.OrderId,
                    position.position.CauseOrderPositionId,
                    position.position.CausePackagePositionId,
                    position.position.CauseItemPositionId,

                    position.position.PrincipalPositionId,

                    position.Start,
                    position.OrganizationUnitId,
                }).Distinct(); // схлопнем позиции по всем нарушенным правилам

            var messages = from conflict in notSatisfiedPositions
                           join period in query.For<Period>() on new { conflict.Start, conflict.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                           select new Version.ValidationResult
                           {
                               VersionId = version,
                               MessageType = MessageTypeId,
                               MessageParams =
                                    new XDocument(new XElement("element",
                                                               new XAttribute("firm", conflict.FirmId),
                                                               new XElement("position",
                                                                            new XAttribute("orderId", conflict.OrderId),
                                                                            new XAttribute("orderNumber", query.For<Order>().Single(x => x.Id == conflict.OrderId).Number),
                                                                            new XAttribute("orderPositionId", conflict.CauseOrderPositionId),
                                                                            new XAttribute("orderPositionName", query.For<Position>().Single(x => x.Id == conflict.CausePackagePositionId).Name),
                                                                            new XAttribute("positionId", conflict.CauseItemPositionId),
                                                                            new XAttribute("positionName", query.For<Position>().Single(x => x.Id == conflict.CauseItemPositionId).Name)),
                                                               new XElement("position",
                                                                            new XAttribute("orderId", conflict.OrderId),
                                                                            new XAttribute("orderNumber", query.For<Order>().Single(x => x.Id == conflict.OrderId).Number),
                                                                            new XAttribute("positionId", conflict.PrincipalPositionId),
                                                                            new XAttribute("positionName", query.For<Position>().Single(x => x.Id == conflict.PrincipalPositionId).Name))
                                                              )
                                                 ),

                               PeriodStart = period.Start,
                               PeriodEnd = period.End,
                               ProjectId = period.ProjectId,

                               ReferenceType = EntityTypeIds.Order,
                               ReferenceId = conflict.OrderId,

                               Result = RuleResult,
                           };

            return messages;
        }
    }
}
