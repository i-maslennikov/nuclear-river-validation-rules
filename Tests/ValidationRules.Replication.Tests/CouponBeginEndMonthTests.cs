using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    public sealed class CouponBeginEndMonthTests
    {
        [TestCaseSource(nameof(BeginMonthTestCases))]
        public void BeginMonthTests(DateTime actual, DateTime expected)
        {
            Assert.AreEqual(expected, AdvertisementAggregateRootActor.CouponAccessor.BeginMonth(actual));
        }
        private static IEnumerable<TestCaseData> BeginMonthTestCases()
        {
            return new[]
                {
                    new TestCaseData(Case(27, 01), Case(01, 01)),

                    new TestCaseData(Case(31, 01), Case(01, 02)),
                    new TestCaseData(Case(01, 02), Case(01, 02)),
                    new TestCaseData(Case(05, 02), Case(01, 02)),
                    new TestCaseData(Case(06, 02), Case(01, 02)),
                    new TestCaseData(Case(24, 02), Case(01, 02)),
                };
        }

        [TestCaseSource(nameof(EndMonthTestCases))]
        public void EndMonthTests(DateTime actual, DateTime expected)
        {
            Assert.AreEqual(expected, AdvertisementAggregateRootActor.CouponAccessor.EndMonth(actual));
        }
        private static IEnumerable<TestCaseData> EndMonthTestCases()
        {
            return new[]
                {
                    new TestCaseData(Case(01, 01), Case(01, 01)),
                    new TestCaseData(Case(04, 01), Case(01, 01)),

                    new TestCaseData(Case(05, 01), Case(01, 02)),
                    new TestCaseData(Case(31, 01), Case(01, 02)),
                };
        }

        private static DateTime Case(int day, int month)
        {
            return new DateTime(10, month, day);
        }
    }
}