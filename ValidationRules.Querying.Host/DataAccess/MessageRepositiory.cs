using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public class MessageRepositiory
    {
        private const string ConfigurationString = "Messages";

        private readonly DataConnectionFactory _factory;

        public MessageRepositiory(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public bool TryGetVersion(Guid state, out long versionId)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var stateReached = connection.GetTable<Version.ErmState>().SingleOrDefault(x => x.Token == state);
                versionId = stateReached?.VersionId ?? 0;
                return stateReached != null;
            }
        }

        public long GetLatestVersion()
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                return connection.GetTable<Version.ValidationResult>().Max(x => x.VersionId);
            }
        }

        public IReadOnlyCollection<Message> GetMessages(long versionId, IReadOnlyCollection<long> orderIds, long? projectId, DateTime start, DateTime end, ResultType resultType)
        {
            var dateFilter = CreateDateFilter(start, end);

            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var validationResults = GetValidationResult(connection.GetTable<Version.ValidationResult>().Where(x => x.VersionId <= versionId));

                var resultsByOrder = validationResults.Where(dateFilter).Where(CreateOrderFilter(orderIds));
                var resultsByProject = validationResults.Where(dateFilter).Where(CreateProjectFilter(projectId));

                var query = resultsByOrder.Concat(resultsByProject).Where(x => (x.Result & resultType.ToBitMask()) != 0);

                return query.ToMessages(resultType).ToList();
            }
        }

        private static IQueryable<Version.ValidationResult> GetValidationResult(IQueryable<Version.ValidationResult> query)
        {
            // если выше по стеку нашли resolved результаты, то отфильтровываем их
            return from vr in query.Where(x => !x.Resolved)
                   where !query.Any(x => x.VersionId > vr.VersionId &&
                                           x.Resolved &&

                                           x.MessageType == vr.MessageType &&
                                           x.MessageParams == vr.MessageParams &&
                                           x.PeriodStart == vr.PeriodStart &&
                                           x.PeriodEnd == vr.PeriodEnd &&
                                           x.ProjectId == vr.ProjectId &&
                                           x.OrderId == vr.OrderId &&
                                           x.Result == vr.Result)
                   select vr;
        }

        private static Expression<Func<Version.ValidationResult, bool>> CreateDateFilter(DateTime start, DateTime end)
        {
            return x => x.PeriodStart < end && start < x.PeriodEnd;
        }

        private static Expression<Func<Version.ValidationResult, bool>> CreateOrderFilter(IReadOnlyCollection<long> orderIds)
        {
            if (orderIds.Any())
                return x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value);

            return x => false;
        }

        private static Expression<Func<Version.ValidationResult, bool>> CreateProjectFilter(long? projectId)
        {
            if (projectId.HasValue)
                return x => x.ProjectId.HasValue && x.ProjectId == projectId.Value;

            return x => false;
        }
    }
}