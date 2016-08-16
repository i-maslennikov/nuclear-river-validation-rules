using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для прайс-листов, у позиций которых более одной AssociatedPositionsGroup должно выводиться предупреждение.
    /// "В Позиции прайс-листа {0} содержится более одной группы сопутствующих позиций, что не поддерживается системой."
    /// </summary>
    public sealed class AssociatedPositionsGroupCountActor : IActor
    {
        public const int MessageTypeId = 7;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Warning)
                                                                    .WhenMassRelease(Result.Warning);

        private readonly ValidationRuleShared _validationRuleShared;

        public AssociatedPositionsGroupCountActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var ruleResults = from overcount in query.For<AssociatedPositionGroupOvercount>()
                              join price in query.For<Price>() on overcount.PriceId equals price.Id
                              join pp in query.For<PricePeriod>() on overcount.PriceId equals pp.PriceId
                              join period in query.For<Period>() on new { pp.Start, pp.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                              join project in query.For<Project>() on period.ProjectId equals project.Id
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("price",
                                                                                              new XAttribute("id", price.Id),
                                                                                              new XAttribute("beginDate", price.BeginDate)),
                                                                                 new XElement("project",
                                                                                              new XAttribute("id", project.Id),
                                                                                              new XAttribute("name", project.Name)),
                                                                                 new XElement("pricePosition",
                                                                                              new XAttribute("id", overcount.PricePositionId),
                                                                                              new XAttribute("name", overcount.PricePositionName)))),
                                      PeriodStart = period.Start,
                                      PeriodEnd = period.End,
                                      ProjectId = period.ProjectId,
                                      VersionId = version,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
