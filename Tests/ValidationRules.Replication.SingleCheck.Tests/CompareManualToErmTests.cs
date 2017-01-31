using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;
using LinqToDB.Mapping;

using Newtonsoft.Json;

using NUnit.Framework;

using ValidationRules.Replication.SingleCheck.Tests.ErmService;
using ValidationRules.Replication.SingleCheck.Tests.RiverService;

namespace ValidationRules.Replication.SingleCheck.Tests
{
    [Ignore]
    [TestFixture]
    public sealed class CompareManualToErmTests
    {
        private readonly RiverToErmResultAdapter _riverService = new RiverToErmResultAdapter("River");
        private readonly OrderValidationApplicationServiceClient _ermService = new OrderValidationApplicationServiceClient("Erm");

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
                          .ToArray();

                    foreach (var release in releases)
                    {
                        result.Add(new TestCaseData(release.OrganizationUnitId, release.NextRelease));
                    }
                }

                return result;
            }
        }

        [TestCaseSource(nameof(Releases))]
        public void TestRules(long organizationUnitId, DateTime releaseDate)
        {
            var riverTime = Stopwatch.StartNew();
            var riverResult = InvokeRiver(organizationUnitId, releaseDate);
            riverTime.Stop();

            var ermTime = Stopwatch.StartNew();
            var ermResult = InvokeErm(organizationUnitId, releaseDate);
            ermTime.Stop();

            var diff = riverResult
                .Keys
                .Select(x => new { Key = x, River = TryGetSorted(riverResult, x), Erm = TryGetSorted(ermResult, x) })
                .ToDictionary(x => x.Key, x => new RuleReport(x.River, x.Erm))
                .ToArray();

            Assert.True(diff.All(x => x.Value.OnlyErm + x.Value.OnlyRiver == 0), JsonConvert.SerializeObject(diff, Formatting.Indented));
            Assert.Pass($"River: {riverTime.ElapsedMilliseconds}, Erm: {ermTime.ElapsedMilliseconds}");
        }

        private IDictionary<int, Tuple<long, string>[]> InvokeRiver(long organizationUnitId, DateTime releaseDate)
        {
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

            return _riverService.ValidateMassManual(orderIds, projectId, releaseDate)
                                .Messages
                                .GroupBy(x => x.RuleCode.ToErmRuleCode(), x => Tuple.Create(x.TargetEntityId, x.MessageText))
                                .ToDictionary(x => x.Key, x => x.ToArray());
        }

        private IDictionary<int, Tuple<long, string>[]> InvokeErm(long organizationUnitId, DateTime releaseDate)
        {
            var request = new ValidateOrdersRequest(ValidationType.ManualReportWithAccountsCheck,
                                                    organizationUnitId,
                                                    new TimePeriod { Start = releaseDate, End = releaseDate.AddMonths(1).AddSeconds(-1) },
                                                    null,
                                                    false);
            return _ermService.ValidateOrders(request).ValidateOrdersResult.Messages
                              .GroupBy(x => x.RuleCode, x => Tuple.Create(x.TargetEntityId, x.MessageText))
                              .ToDictionary(x => x.Key, x => x.ToArray());
        }

        private Tuple<long, string>[] TryGetSorted(IDictionary<int, Tuple<long, string>[]> result, int key)
        {
            Tuple<long, string>[] value;
            result.TryGetValue(key, out value);
            return value ?? Array.Empty<Tuple<long, string>>();
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
