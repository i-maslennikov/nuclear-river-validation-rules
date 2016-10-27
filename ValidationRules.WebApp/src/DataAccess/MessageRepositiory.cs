using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public class MessageRepositiory
    {
        private readonly DataConnectionFactory _factory;

        public MessageRepositiory(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public IReadOnlyCollection<ValidationResult> GetMessages(IReadOnlyCollection<long> orderIds, long? projectId)
        {
            return InBatches(orderIds, projectId, 300);
        }

        private IReadOnlyCollection<ValidationResult> InBatches(IReadOnlyCollection<long> orderIds, long? projectId, int batchSize)
        {
            return Enumerable.Range(0, 1 + orderIds.Count / batchSize).AsParallel().SelectMany(batchNumber => GetMessages(orderIds, projectId, batchSize, batchNumber)).ToArray();
        }

        private IReadOnlyCollection<ValidationResult> GetMessages(IReadOnlyCollection<long> orderIds, long? projectId, int batchSize, int batchNumber)
        {
            using (var connection = _factory.CreateDataConnection("Messages"))
            {
                var results = connection.GetTable<ValidationResult>()
                    .Where(x => orderIds.Skip(batchNumber * batchSize).Take(batchSize).Contains(x.OrderId))
                    .Where(CreateProjectFilter(projectId));

                return results.ToArray();
            }
        }

        private Expression<Func<ValidationResult, bool>> CreateProjectFilter(long? projectId)
        {
            if (projectId.HasValue)
                return x => x.ProjectId == projectId.Value;
            return x => true;
        }
    }
}