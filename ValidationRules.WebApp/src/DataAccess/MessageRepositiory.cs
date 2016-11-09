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
                var validationResults = GetValidationResult(connection.GetTable<ValidationResult>());

                var resultsByOrder = validationResults.Where(dateFilter).Where(CreateOrderFilter(orderIds));
                var resultsByProject = validationResults.Where(dateFilter).Where(CreateProjectFilter(projectId));

                return resultsByOrder.Concat(resultsByProject).ToArray();
            }
        }

        private static IQueryable<ValidationResult> GetValidationResult(IQueryable<ValidationResult> query)
        {
            // если выше по стеку нашли resolved результаты, то отфильтровываем их
            return from vr in query.Where(x => !x.Resolved)
                    where !query.Any(x => x.VersionId > vr.VersionId &&
                                            x.Resolved &&

                                            x.MessageType == vr.MessageType &&
                                            x.MessageParams == vr.MessageParams &&
                                            x.PeriodStart == vr.PeriodStart &&
                                            x.ProjectId == vr.ProjectId &&
                                            x.OrderId == vr.OrderId &&
                                            x.Result == vr.Result)
                    select vr;
        }

        private static Expression<Func<ValidationResult, bool>> CreateDateFilter(DateTime start, DateTime end)
        {
            return x => x.PeriodStart < end && start < x.PeriodEnd;
        }

        private static Expression<Func<ValidationResult, bool>> CreateOrderFilter(IReadOnlyCollection<long> orderIds)
        {
            if (orderIds.Any())
                return x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value);

            return x => false;
        }

        private static Expression<Func<ValidationResult, bool>> CreateProjectFilter(long? projectId)
        {
            if (projectId.HasValue)
                return x => x.ProjectId.HasValue && x.ProjectId == projectId.Value;

            return x => false;
        }
    }
}