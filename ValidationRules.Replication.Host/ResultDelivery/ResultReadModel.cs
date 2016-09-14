using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Storage.API.Readings;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultReadModel
    {
        private readonly IQuery _query;

        public ResultReadModel(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Message> GetResults(IEnumerable<Tuple<long, DateTime>> filter)
        {
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var lastVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault();
                var lastVersionId = lastVersion?.Id ?? 0;
                var results = new List<Message>();
                foreach (var group in filter.GroupBy(x => x.Item2, x => x.Item1))
                {
                    var query = _query.For<Version.ValidationResultByOrder>()
                                      .Where(x => x.VersionId == lastVersionId)
                                      .Where(x => x.PeriodStart <= group.Key && group.Key < x.PeriodEnd)
                                      .Where(x => group.Contains(x.OrderId))
                                      .Select(x => new Message((MessageTypeCode)x.MessageType, x.MessageParams, x.Result));
                    results.AddRange(query);
                }

                transaction.Complete();
                return results;
            }
        }
    }
}