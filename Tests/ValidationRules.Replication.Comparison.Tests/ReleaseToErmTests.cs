using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB.Data;
using LinqToDB.Mapping;

using Newtonsoft.Json;

using NUnit.Framework;

using ValidationRules.Replication.Comparison.Tests.ErmService;
using ValidationRules.Replication.Comparison.Tests.RiverService;

namespace ValidationRules.Replication.Comparison.Tests
{
    [TestFixture]
    public sealed class ReleaseToErmTests
    {
        private readonly RiverToErmResultAdapter _riverService = new RiverToErmResultAdapter("River");
        private readonly ErmToRiverResultAdapter _ermService = new ErmToRiverResultAdapter("Erm");

        public IReadOnlyCollection<TestCaseData> Releases
        {
            get
            {
                var result = new List<TestCaseData>();
                using (var dc = new DataConnection("Erm"))
                {
                    var releases =
                        dc.GetTable<ReleaseInfo>().Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == 2)
                          .GroupBy(x => x.OrganizationUnitId)
                          .Select(x => new { OrganizationUnitId = x.Key, NextRelease = x.Select(y => y.PeriodEndDate).Max().AddSeconds(1) })
                          .Where(x => dc.GetTable<Project>().Any(project => project.IsActive && project.OrganizationUnitId == x.OrganizationUnitId))
                          //.Take(1)
                          .ToArray();

                    foreach (var release in releases)
                    {
                        result.Add(new TestCaseData(release.OrganizationUnitId, release.NextRelease));
                    }
                }

                return result;
            }
        }

        [Category("CronDaily")]
        [Category("Mass")]
        [Category("Release")]
        [TestCaseSource(nameof(Releases))]
        public async Task TestRelease(long organizationUnitId, DateTime releaseDate)
        {
            var riverValidation = Task.Run(() => InvokeRiver(organizationUnitId, releaseDate));
            var ermValidation = Task.Run(() => InvokeErm(organizationUnitId, releaseDate));

            await Task.WhenAll(riverValidation, ermValidation);

            var riverValidationSummary = await riverValidation;
            var ermValidationSummary = await ermValidation;

            var diff = riverValidationSummary.Results
                                             .Keys
                                             .Union(ermValidationSummary.Results.Keys)
                                             .Select(ruleCode => new
                                                 {
                                                     RuleCode = ruleCode,
                                                     River = riverValidationSummary.Results
                                                                                   .TryGetValue(ruleCode, out var riverRuleResult) && riverRuleResult != null
                                                                 ? riverRuleResult
                                                                 : Array.Empty<Tuple<long, string>>(),
                                                     Erm = ermValidationSummary.Results
                                                                               .TryGetValue(ruleCode, out var ermRuleResult) && ermRuleResult != null
                                                               ? ermRuleResult
                                                               : Array.Empty<Tuple<long, string>>()
                                                 })
                                             .OrderBy(x => x.RuleCode)
                                             .ToDictionary(x => x.RuleCode, x => new RuleReport(x.River, x.Erm))
                                             .ToArray();

            Assert.True(diff.All(x => x.Value.OnlyErm + x.Value.OnlyRiver == 0), JsonConvert.SerializeObject(diff, Formatting.Indented));
            Assert.Pass($"River: {riverValidationSummary.ItTakes.TotalMilliseconds}, Erm: {ermValidationSummary.ItTakes.TotalMilliseconds}");
        }

        private ValidationSessionSummary InvokeRiver(long organizationUnitId, DateTime releaseDate)
        {
            var itTakes = Stopwatch.StartNew();

            long[] orderIds;
            long projectId;
            using (var dc = new DataConnection("Erm"))
            {
                orderIds = dc.GetTable<Order>().Where(x => x.IsActive && !x.IsDeleted)
                             .Where(x => x.DestOrganizationUnitId == organizationUnitId || x.SourceOrganizationUnitId == organizationUnitId)
                             .Where(x => x.WorkflowStepId == 4 || x.WorkflowStepId == 5)
                             .Where(x => x.BeginDistributionDate <= releaseDate && x.EndDistributionDateFact >= releaseDate)
                             .Select(x => x.Id)
                             .ToArray();
                projectId = dc.GetTable<Project>().Where(x => x.IsActive && x.OrganizationUnitId == organizationUnitId).Select(x => x.Id).Single();
            }

            var sessionResults = _riverService.ValidateMassRelease(orderIds, projectId, releaseDate)
                                              .Messages
                                              .GroupBy(x => x.RuleCode, x => Tuple.Create(x.TargetEntityId, x.MessageText))
                                              .Where(x => x.Key != 0)
                                              .ToDictionary(x => x.Key, x => x.ToArray());

            return new ValidationSessionSummary
                {
                    Results = sessionResults,
                    ItTakes = itTakes.Elapsed
                };
        }

        private ValidationSessionSummary InvokeErm(long organizationUnitId, DateTime releaseDate)
        {
            var itTakes = Stopwatch.StartNew();
            var sessionResults = _ermService.ValidateMassRelease(organizationUnitId, releaseDate).Messages
                                            .GroupBy(x => x.RuleCode.CoerceFromErmRuleCode(), x => Tuple.Create(x.TargetEntityId, x.MessageText))
                                            .Where(x => x.Key != 0)
                                            .ToDictionary(x => x.Key, x => x.ToArray());

            return new ValidationSessionSummary
                {
                    Results = sessionResults,
                    ItTakes = itTakes.Elapsed
                };
        }

        private sealed class ValidationSessionSummary
        {
            public IDictionary<int, Tuple<long, string>[]> Results { get; set; }
            public TimeSpan ItTakes { get; set; }
        }

        private class RuleReport
        {
            public RuleReport(IReadOnlyCollection<Tuple<long, string>> river, IReadOnlyCollection<Tuple<long, string>> erm)
            {
                var onlyRiver = river.Except(erm).ToArray();
                var onlyErm = erm.Except(river).ToArray();
                OnlyRiver = onlyRiver.Length;
                OnlyErm = onlyErm.Length;
                Common = river.Intersect(erm).Count();

                River = onlyRiver.OrderBy(x => x.Item2).Take(3).ToArray();
                Erm = onlyErm.OrderBy(x => x.Item2).Take(3).ToArray();
            }

            public int OnlyRiver { get; }
            public int OnlyErm { get; }
            public int Common { get; }

            public IReadOnlyCollection<Tuple<long, string>> River { get; }
            public IReadOnlyCollection<Tuple<long, string>> Erm { get; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
        }

        [Table(Name = "ReleaseInfos", Schema = "Billing")]
        public sealed class ReleaseInfo
        {
            [Column] public long OrganizationUnitId { get; set; }
            [Column] public bool IsBeta { get; set; }
            [Column] public int Status { get; set; }
            [Column] public bool IsActive { get; set; }
            [Column] public bool IsDeleted { get; set; }
            [Column] public DateTime PeriodEndDate { get; set; }
        }

        [Table(Name = "Orders", Schema = "Billing")]
        public sealed class Order
        {
            [Column] public long Id { get; set; }
            [Column] public long DestOrganizationUnitId { get; set; }
            [Column] public long SourceOrganizationUnitId { get; set; }
            [Column] public DateTime BeginDistributionDate { get; set; }
            [Column] public DateTime EndDistributionDateFact { get; set; }
            [Column] public int WorkflowStepId { get; set; }
            [Column] public bool IsActive { get; set; }
            [Column] public bool IsDeleted { get; set; }
        }

        [Table(Name = "Projects", Schema = "Billing")]
        public sealed class Project
        {
            [Column] public long Id { get; set; }
            [Column] public long? OrganizationUnitId { get; set; }
            [Column] public bool IsActive { get; set; }
        }
    }
}
