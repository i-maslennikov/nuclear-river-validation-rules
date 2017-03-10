using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyCollection<Message> GetMessages(long versionId, IReadOnlyCollection<long> orderIds, long? projectId, DateTime start, DateTime end, ResultType resultType)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var validationResults = connection.GetTable<Version.ValidationResult>()
                                                  .ForOrdersOrProject(orderIds, projectId)
                                                  .ForPeriod(start, end)
                                                  .ForVersion(versionId);

                var validationResultTypes = connection.GetTable<Version.ValidationResultType>()
                                                  .Where(x => x.ResultType == resultType);

                var messages = from validationResult in validationResults
                               from validationResultType in validationResultTypes
                                                            .Where(x => x.MessageType == validationResult.MessageType)
                               select validationResult.ToMessage(validationResultType.Result);

                return messages.ToList();
            }
        }
    }
}