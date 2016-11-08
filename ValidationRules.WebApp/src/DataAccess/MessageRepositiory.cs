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

        public IReadOnlyCollection<ValidationResult> GetMessages(IReadOnlyCollection<long> orderIds, long? projectId, DateTime start, DateTime end)
        {
            var dateFilter = CreateDateFilter(start, end);

            using (var connection = _factory.CreateDataConnection("Messages"))
            {
                var resultsByOrder = connection.GetTable<ValidationResult>().Where(dateFilter).Where(CreateOrderFilter(orderIds));
                var resultsByProject = connection.GetTable<ValidationResult>().Where(dateFilter).Where(CreateProjectFilter(projectId));

                return resultsByOrder.Concat(resultsByProject).ToArray();
            }
        }

        private Expression<Func<ValidationResult, bool>> CreateDateFilter(DateTime start, DateTime end)
        {
            return x => x.PeriodStart < end && start < x.PeriodEnd;
        }

        private Expression<Func<ValidationResult, bool>> CreateOrderFilter(IReadOnlyCollection<long> orderIds)
        {
            if (orderIds.Any())
                return x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value);

            return x => false;
        }

        private Expression<Func<ValidationResult, bool>> CreateProjectFilter(long? projectId)
        {
            if (projectId.HasValue)
                return x => x.ProjectId.HasValue && x.ProjectId == projectId.Value;

            return x => false;
        }
    }
}