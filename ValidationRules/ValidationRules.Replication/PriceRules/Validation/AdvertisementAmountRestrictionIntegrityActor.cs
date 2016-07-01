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
    /// Для прайс-листов, в которых для позиций с контроллируемым количеством не указан минимум должна выводиться ошибка.
    /// </summary>
    public sealed class AdvertisementAmountRestrictionIntegrityActor : IActor
    {
        private const int MessageTypeId = 2;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public AdvertisementAmountRestrictionIntegrityActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var ruleResults = from restriction in query.For<AdvertisementAmountRestriction>().Where(x => x.MissingMinimalRestriction)
                              join pp in query.For<PricePeriod>() on restriction.PriceId equals pp.PriceId
                              join period in query.For<Period>() on new { pp.Start, pp.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                              join op in query.For<OrderPeriod>() on new { pp.Start, pp.OrganizationUnitId } equals new { op.Start, op.OrganizationUnitId }
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                      MessageParams = new XDocument(new XElement("empty", new XAttribute("name", restriction.CategoryName))),
                                      PeriodStart = period.Start,
                                      PeriodEnd = period.End,
                                      ProjectId = period.ProjectId,
                                      VersionId = version,

                                      ReferenceType = EntityTypeIds.Project,
                                      ReferenceId = period.ProjectId,

                                      Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
