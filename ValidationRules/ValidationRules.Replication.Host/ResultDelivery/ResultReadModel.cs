using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultReadModel
    {
        private const int OrderReferenceType = 151;

        private readonly IQuery _query;

        public ResultReadModel(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Version.ValidationResult> GetResults(IEnumerable<Tuple<long, DateTime>> filter)
        {
            // Просто .First() приводит к сообщению, что подключение закрыто.
            var lastVersion = _query.For<Version>().OrderByDescending(x => x.Id).Take(1).ToArray().First();

            var results = new List<Version.ValidationResult>();
            foreach (var group in filter.GroupBy(x => x.Item2, x => x.Item1))
            {
                var query = _query.For<Version.ValidationResult>()
                                  .Where(x => x.VersionId == lastVersion.Id)
                                  .Where(x => x.PeriodStart <= group.Key && group.Key < x.PeriodEnd)
                                  .Where(x => x.ReferenceType == OrderReferenceType)
                                  .Where(x => group.Contains(x.ReferenceId));
                results.AddRange(query);
            }

            return results;
        }
    }
}