using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class MessageRepositiory
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

        public IReadOnlyCollection<Version.ValidationResult> GetMessages(long versionId, IReadOnlyCollection<long> orderIds, long? projectId, DateTime start, DateTime end, ICheckModeDescriptor checkModeDescriptor)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var validationResults = connection.GetTable<Version.ValidationResult>()
                                                  .Where(ForOrdersOrProject(orderIds, projectId))
                                                  .Where(ForPeriod(start, end))
                                                  .Where(ForMode(checkModeDescriptor))
                                                  .ForVersion(versionId);

                return validationResults.ToList();
            }
        }

        private static Expression<Func<Version.ValidationResult, bool>> ForPeriod(DateTime start, DateTime end)
            => x => x.PeriodStart < end && start < x.PeriodEnd;

        private static Expression<Func<Version.ValidationResult, bool>> ForOrdersOrProject(IReadOnlyCollection<long> orderIds, long? projectId)
            => x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) || x.ProjectId.HasValue && x.ProjectId == projectId;

        private static Expression<Func<Version.ValidationResult, bool>> ForMode(ICheckModeDescriptor checkModeDescriptor)
            => x => checkModeDescriptor.Rules.Contains((MessageTypeCode)x.MessageType);
    }
}