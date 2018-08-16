using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB.Data;
using LinqToDB.Mapping;

using Newtonsoft.Json;

using NUnit.Framework;

using ValidationRules.Replication.Comparison.Tests.RiverService;

namespace ValidationRules.Replication.Comparison.Tests
{
    [TestFixture]
    public sealed class ReleaseToErmTests
    {
        private readonly RiverToErmResultAdapter _testRiverService = new RiverToErmResultAdapter("TestEnvironment");
        private readonly RiverToErmResultAdapter _etalonRiverService = new RiverToErmResultAdapter("EtalonEnvironment");

        public static IReadOnlyCollection<TestCaseData> Releases()
        {
            var result = new List<TestCaseData>();
            using (var dc = new DataConnection("Erm"))
            {
                var releases =
                    dc.GetTable<ReleaseInfo>().Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == 2)
                      .GroupBy(x => x.OrganizationUnitId)
                      .Select(x => new { OrganizationUnitId = x.Key, NextRelease = x.Select(y => y.PeriodEndDate).Max().AddSeconds(1) })
                      .Where(x => dc.GetTable<Project>().Any(project => project.IsActive && project.OrganizationUnitId == x.OrganizationUnitId))
                      .ToArray();

                foreach (var release in releases)
                {
                    result.Add(new TestCaseData(release.OrganizationUnitId, release.NextRelease));
                }
            }

            return result;
        }

        [Category("CronDaily")]
        [Category("Mass")]
        [Category("Release")]
        [TestCaseSource(nameof(Releases))]
        [Parallelizable(ParallelScope.All)]
        public async Task TestRelease(long organizationUnitId, DateTime releaseDate)
        {
            TestContext.Progress.WriteLine($"Strated. organizationUnitId: {organizationUnitId}. releaseDate: {releaseDate}{Environment.NewLine}");

            var testRiverValidation = Task.Run(() => InvokeRiver(_testRiverService, organizationUnitId, releaseDate));
            var etalonRiverValidation = Task.Run(() => InvokeRiver(_etalonRiverService, organizationUnitId, releaseDate));

            await Task.WhenAll(testRiverValidation, etalonRiverValidation);

            var testRiverValidationSummary = await testRiverValidation;
            var etalonRiverValidationSummary = await etalonRiverValidation;

            var diff = testRiverValidationSummary.Results
                                                 .Keys
                                                 .Union(etalonRiverValidationSummary.Results.Keys)
                                                 .Select(ruleCode => new
                                                     {
                                                         RuleCode = ruleCode,
                                                         TestEnvironment = testRiverValidationSummary.Results
                                                                                                     .TryGetValue(ruleCode, out var riverRuleResult) && riverRuleResult != null
                                                                               ? riverRuleResult
                                                                               : Array.Empty<Tuple<long, string>>(),
                                                         EtalonEnvironment = etalonRiverValidationSummary.Results
                                                                                                         .TryGetValue(ruleCode, out var ermRuleResult) && ermRuleResult != null
                                                                                 ? ermRuleResult
                                                                                 : Array.Empty<Tuple<long, string>>()
                                                     })
                                                 .OrderBy(x => x.RuleCode)
                                                 .ToDictionary(x => x.RuleCode, x => new RuleReport(x.TestEnvironment, x.EtalonEnvironment))
                                                 .ToArray();

            TestContext.Progress.WriteLine($"Finished. organizationUnitId: {organizationUnitId}. releaseDate: {releaseDate}{Environment.NewLine}");
            TestContext.Progress.WriteLine($"TestEnvironment: {testRiverValidationSummary.ItTakes.TotalMilliseconds}, EtalonEnvironment: {etalonRiverValidationSummary.ItTakes.TotalMilliseconds}");
            Assert.True(diff.All(x => x.Value.OnlyErm + x.Value.OnlyRiver == 0), JsonConvert.SerializeObject(diff, Formatting.Indented));
        }

        private ValidationSessionSummary InvokeRiver(RiverToErmResultAdapter riverService, long organizationUnitId, DateTime releaseDate)
        {
            var itTakes = Stopwatch.StartNew();

            long[] orderIds;
            long projectId;
            using (var dc = new DataConnection("Erm"))
            {
                orderIds = dc.GetTable<Order>()
                             .Where(x => x.IsActive && !x.IsDeleted)
                             .Where(x => x.DestOrganizationUnitId == organizationUnitId || x.SourceOrganizationUnitId == organizationUnitId)
                             .Where(x => x.WorkflowStepId == 4 || x.WorkflowStepId == 5)
                             .Where(x => x.BeginDistributionDate <= releaseDate && x.EndDistributionDateFact >= releaseDate)
                             .Select(x => x.Id)
                             .ToArray();
                projectId = dc.GetTable<Project>()
                              .Where(x => x.IsActive && x.OrganizationUnitId == organizationUnitId)
                              .Select(x => x.Id)
                              .Single();
            }

            var sessionResults = riverService.ValidateMassRelease(orderIds, projectId, releaseDate)
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

        private sealed class ValidationSessionSummary
        {
            public IDictionary<int, Tuple<long, string>[]> Results { get; set; }
            public TimeSpan ItTakes { get; set; }
        }

        private class RuleReport
        {
            public RuleReport(IReadOnlyCollection<Tuple<long, string>> resultsFromTestEnvironment, IReadOnlyCollection<Tuple<long, string>> resultsFromEtalonEnvironment)
            {
                var onlyTest = resultsFromTestEnvironment.Except(resultsFromEtalonEnvironment)
                                                         .ToList();
                var onlyEtalon = resultsFromEtalonEnvironment.Except(resultsFromTestEnvironment)
                                                             .ToList();
                OnlyRiver = onlyTest.Count;
                OnlyErm = onlyEtalon.Count;
                Common = resultsFromTestEnvironment.Intersect(resultsFromEtalonEnvironment).Count();

                TestEnvironment = onlyTest.OrderBy(x => x.Item2)
                                .Take(3)
                                .ToArray();
                EtalonEnvironment = onlyEtalon.OrderBy(x => x.Item2)
                                              .Take(3)
                                              .ToArray();
            }

            public int OnlyRiver { get; }
            public int OnlyErm { get; }
            public int Common { get; }

            public IReadOnlyCollection<Tuple<long, string>> TestEnvironment { get; }
            public IReadOnlyCollection<Tuple<long, string>> EtalonEnvironment { get; }

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
