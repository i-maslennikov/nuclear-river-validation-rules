using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers.Utils
{
    internal static class PeriodUtils
    {
        public static IReadOnlyCollection<DateTime> ExtractPeriods(this IReadOnlyDictionary<string, string> extra)
        {
            return FromString(extra["periods"]);
        }

        public static Dictionary<string, string> StorePeriods(this IReadOnlyCollection<Message> messages, Dictionary<string, string> extra)
        {
            extra.Add("periods", messages.Select(x => x.Extra).SelectMany(MonthlySplit).Distinct().ConvertToString());
            return extra;
        }

        private static IReadOnlyCollection<DateTime> FromString(string str)
            => JsonConvert.DeserializeObject<DateTime[]>(str);

        private static IEnumerable<DateTime> MonthlySplit(IReadOnlyDictionary<string, string> extra)
            => MonthlySplit(extra["begin"], extra["end"]);

        private static IEnumerable<DateTime> MonthlySplit(string begin, string end)
            => MonthlySplit(DateTime.Parse(begin), DateTime.Parse(end));

        private static IEnumerable<DateTime> MonthlySplit(DateTime begin, DateTime end)
        {
            for (var x = begin; x < end; x = x.AddMonths(1))
            {
                yield return x;
            }
        }

        private static string ConvertToString(this IEnumerable<DateTime> periods)
            => JsonConvert.SerializeObject(periods);
    }
}