using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Messages;

using NUnit.Framework;

using ValidationRules.Replication.SingleCheck.Tests.ErmService;

using ValidationRules.Replication.SingleCheck.Tests.RiverService;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace ValidationRules.Replication.SingleCheck.Tests
{
    [Ignore]
    [TestFixture]
    public sealed class CompareSingleToErmTests
    {
        private readonly RiverToErmResultAdapter _riverService = new RiverToErmResultAdapter("River");
        private readonly ErmToRiverResultAdapter _ermService = new ErmToRiverResultAdapter("Erm");

        public IReadOnlyCollection<TestCaseData> Rules
        {
            get
            {
                var rules = Enum.GetValues(typeof(MessageTypeCode)).Cast<int>().ToArray();
                var result = new List<TestCaseData>(rules.Length);

                using (var dc = new DataConnection("ReferenceSource").AddMappingSchema(Schema.Messages))
                {
                    var orderErrors = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == false && x.OrderId.HasValue);
                    var resolved = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == true);
                    var results =
                        from message in orderErrors
                        where !resolved.Any(x => x.MessageType == message.MessageType && x.OrderId == message.OrderId && x.VersionId > message.VersionId)
                        select message;

                    foreach (var rule in rules)
                    {
                        var error = results.FirstOrDefault(x => x.MessageType == rule && x.OrderId.HasValue);
                        result.Add(new TestCaseData(rule, error?.OrderId));
                    }
                }

                return result;
            }
        }

        [TestCaseSource(nameof(Rules))]
        public void TestRule(long rule, long? orderId)
        {
            if (!orderId.HasValue)
            {
                Assert.Inconclusive();
            }

            var riverTime = Stopwatch.StartNew();
            var riverResult = InvokeRiver(orderId.Value, rule);
            riverTime.Stop();

            var ermTime = Stopwatch.StartNew();
            var ermResult = InvokeErm(orderId.Value);
            ermTime.Stop();

            var diff = riverResult
                .Keys
                .Select(x => new { Key = x, River = TryGet(riverResult, x), Erm = TryGet(ermResult, x) })
                .Select(x => new { Key = x.Key, OnlyRiver = x.River.Except(x.Erm).ToArray(), OnlyErm = x.Erm.Except(x.River).ToArray(), Common = x.River.Intersect(x.Erm) })
                .Single();

            if (diff.OnlyRiver.Any())
            {
                Assert.Fail($"River messages:\n{string.Join(Environment.NewLine, diff.OnlyRiver)}\nErm messages:\n{string.Join(Environment.NewLine, diff.OnlyErm)}");
            }

            Assert.Pass($"River: {riverTime.ElapsedMilliseconds}, Erm: {ermTime.ElapsedMilliseconds}");
        }

        public IReadOnlyCollection<long> Orders
            => new[] { 958512648357950896 };

        [TestCaseSource(nameof(Orders))]
        public void TestOrder(long orderId)
        {
            var riverResult = InvokeRiver(orderId);
            var ermResult = InvokeErm(orderId);

            var diff = riverResult
                .Keys
                .Union(ermResult.Keys)
                .Select(x => new { Key = x, River = TryGet(riverResult, x), Erm = TryGet(ermResult, x) })
                .Select(x => new { Key = x.Key, OnlyRiver = x.River.Except(x.Erm).ToArray(), OnlyErm = x.Erm.Except(x.River).ToArray() })
                .ToArray();

            Assert.True(diff.All(x => !x.OnlyErm.Any()), "only erm message exist");
            Assert.True(diff.All(x => !x.OnlyRiver.Any()), "only river message exist");
        }

        private IDictionary<int, string[]> InvokeRiver(long orderId, long? rule = null)
            => _riverService.ValidateSingle(orderId).Messages
                            .Where(x => rule == null || x.RuleCode == rule)
                            .GroupBy(x => x.RuleCode.ToErmRuleCode(), x => x.MessageText.TrimEnd('.').Replace("  ", " "))
                            .ToDictionary(x => x.Key, x => x.OrderBy(y => y).ToArray());

        private IDictionary<int, string[]> InvokeErm(long orderId)
            => _ermService.ValidateSingle(orderId).Messages
                          .GroupBy(x => x.RuleCode, x => x.MessageText.Replace(" ,", ",").TrimEnd('.'))
                          .ToDictionary(x => x.Key, x => x.OrderBy(y => y).ToArray());

        private string[] TryGet(IDictionary<int, string[]> result, int key)
        {
            string[] value;
            result.TryGetValue(key, out value);
            return value ?? Array.Empty<string>();
        }
    }
}
