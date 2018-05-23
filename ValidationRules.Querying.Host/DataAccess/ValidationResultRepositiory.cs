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
            => x => x.OrderId == null && x.ProjectId == null || x.OrderId.HasValue && orderIds.Any(y => y.Id == x.OrderId.Value) || x.ProjectId.HasValue && x.ProjectId == projectId;

        private static Expression<Func<Version.ValidationResult, bool>> ForMode(ICheckModeDescriptor checkModeDescriptor)
            => x => checkModeDescriptor.Rules.Keys.Contains((MessageTypeCode)x.MessageType);

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