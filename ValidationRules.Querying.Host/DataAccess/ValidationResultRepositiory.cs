using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Data;

using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class ValidationResultRepositiory
    {
        private const string ConfigurationString = "Messages";

        private readonly DataConnectionFactory _factory;

        public ValidationResultRepositiory(DataConnectionFactory factory)
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

        public IReadOnlyCollection<Version.ValidationResult> GetResults(long versionId, IReadOnlyCollection<long> orderIds, long? projectId, DateTime start, DateTime end, ICheckModeDescriptor checkModeDescriptor)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var orderIdentities = ToTemporaryTable(connection, orderIds);
                var validationResults = connection.GetTable<Version.ValidationResult>()
                                                    .Where(ForOrdersOrProject(orderIdentities, projectId))
                                                    .Where(ForPeriod(start, end))
                                                    .Where(ForMode(checkModeDescriptor))
                                                    .ForVersion(versionId);

                return validationResults.ToList();
            }
        }

        private static Expression<Func<Version.ValidationResult, bool>> ForPeriod(DateTime start, DateTime end)
            => x => x.PeriodStart < end && start < x.PeriodEnd;

        private static Expression<Func<Version.ValidationResult, bool>> ForOrdersOrProject(ITable<Identity> orderIds, long? projectId)
            => x => x.OrderId.HasValue && orderIds.Any(y => y.Id == x.OrderId.Value) || x.ProjectId.HasValue && x.ProjectId == projectId;

        private static Expression<Func<Version.ValidationResult, bool>> ForMode(ICheckModeDescriptor checkModeDescriptor)
            => x => checkModeDescriptor.Rules.Contains((MessageTypeCode)x.MessageType);

        private static ITable<Identity> ToTemporaryTable(DataConnection connection, IEnumerable<long> ids)
        {
            var orderIdentities = connection.CreateTable<Identity>($"#{Guid.NewGuid()}");
            orderIdentities.BulkCopy(ids.Select(x => new Identity { Id = x }));
            return orderIdentities;
        }

        private struct Identity
        {
            public long Id { get; set; }
        }
    }
}