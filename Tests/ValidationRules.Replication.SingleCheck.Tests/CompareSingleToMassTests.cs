using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.SingleCheck.Store;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Messages;

using NUnit.Framework;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace ValidationRules.Replication.SingleCheck.Tests
{
    [Ignore]
    [TestFixture]
    public sealed class CompareSingleToMassTests
    {
        private const int OrderPerRule = 1;

        private static readonly XNodeEqualityComparer Comparer = new XNodeEqualityComparer();

        private readonly PipelineFactory PipelineFactory = new PipelineFactory();

        private IEnumerable<MessageTypeCode> Rules
            => Enum.GetValues(typeof(MessageTypeCode)).Cast<MessageTypeCode>();

        [TestCaseSource(nameof(Orders))]
        public void TestOrder(long orderId)
        {
            using (var dc = new DataConnection("ReferenceSource").AddMappingSchema(Schema.Messages))
            {
                var orderErrors = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == false && x.OrderId.HasValue);
                var resolved = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == true);

                var results =
                    from result in orderErrors.Where(x => x.OrderId == orderId)
                    where !resolved.Any(x => x.MessageType == result.MessageType && x.OrderId == result.OrderId && x.VersionId > result.VersionId)
                    select result;

                var expected = results.ToArray();

                using (var validator = new Validator(PipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", orderId), new PersistentTableStoreFactory("Pipeline"), new HashSetStoreFactory()))
                {
                    var actual = validator.Execute().Where(x => x.OrderId == orderId).ToArray();
                    AssertCollectionsEqual(MergePeriods(expected), MergePeriods(actual));
                }
            }
        }

        private IEnumerable<long> Orders
            => new[] { 997454684204286915, 802519954532144100, 992385830465141559 };

        [TestCaseSource(nameof(Rules))]
        public void TestRule(MessageTypeCode rule)
        {
            using (var dc = new DataConnection("ReferenceSource").AddMappingSchema(Schema.Messages))
            {
                var orderErrors = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == false && x.OrderId.HasValue);
                var resolved = dc.GetTable<Version.ValidationResult>().Where(x => x.Resolved == true);

                var results =
                    from result in orderErrors.Where(x => x.MessageType == (int)rule)
                    where !resolved.Any(x => x.MessageType == result.MessageType && x.OrderId == result.OrderId && x.VersionId > result.VersionId)
                    select result;

                var expecteds = results.GroupBy(x => x.OrderId.Value, x => x).Take(OrderPerRule).ToArray();
                if (expecteds.Length == 0)
                {
                    Assert.Inconclusive();
                }

                foreach (var expected in expecteds)
                {
                    using (var validator = new Validator(PipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", expected.Key), new PersistentTableStoreFactory("Pipeline"), new HashSetStoreFactory()))
                    {
                        var actual = validator.Execute().Where(x => x.OrderId == expected.Key && x.MessageType == (int)rule).ToArray();
                        AssertCollectionsEqual(MergePeriods(expected), MergePeriods(actual));
                    }
                }
            }
        }

        private IReadOnlyDictionary<XNode, List<Tuple<DateTime, DateTime>>> MergePeriods(IEnumerable<Version.ValidationResult> results)
            => results.GroupBy(x => x.MessageParams, x => Tuple.Create(x.PeriodStart, x.PeriodEnd), Comparer)
                      .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Item1).Aggregate(new List<Tuple<DateTime, DateTime>>(), AppendPeriod), Comparer);

        private List<Tuple<DateTime, DateTime>> AppendPeriod(List<Tuple<DateTime, DateTime>> list, Tuple<DateTime, DateTime> period)
        {
            if (!list.Any())
            {
                list.Add(period);
            }
            else
            {
                var last = list.Last();
                if (last.Item2 == period.Item1)
                {
                    list.Remove(last);
                    list.Add(Tuple.Create(last.Item1, period.Item2));
                }
            }

            return list;
        }

        private void AssertCollectionsEqual(IReadOnlyDictionary<XNode, List<Tuple<DateTime, DateTime>>> expected, IReadOnlyDictionary<XNode, List<Tuple<DateTime, DateTime>>> actual)
        {
            var allKeys = expected.Keys.Union(actual.Keys, Comparer);

            foreach (var key in allKeys)
            {
                List<Tuple<DateTime, DateTime>> leftPeriods;
                if (!expected.TryGetValue(key, out leftPeriods))
                {
                    var keys = string.Join(Environment.NewLine, expected.Keys.Select(x => x.ToString(SaveOptions.DisableFormatting)));
                    Assert.Fail($"{key.ToString(SaveOptions.DisableFormatting)}\n is missing in expected, but present:\n {keys}");
                }

                List<Tuple<DateTime, DateTime>> rightPeriods;
                if (!actual.TryGetValue(key, out rightPeriods))
                {
                    var keys = string.Join(Environment.NewLine, actual.Keys.Select(x => x.ToString(SaveOptions.DisableFormatting)));
                    Assert.Fail($"{key.ToString(SaveOptions.DisableFormatting)}\n is missing in actual, but present:\n {keys}");
                }

                Assert.AreEqual(leftPeriods, rightPeriods);
            }
        }
    }
}
