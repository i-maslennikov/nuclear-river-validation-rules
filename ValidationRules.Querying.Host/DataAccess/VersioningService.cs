using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Confluent.Kafka;

using LinqToDB;

using Newtonsoft.Json;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class VersioningService
    {
        private const string ConfigurationString = "Messages";

        private const int ErmWaitAttempts = 36; // 3 mimutes
        private const int AmsWaitAttempts = 12; // 1 mimute
        private static readonly TimeSpan WaitInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan AmsSyncInterval = TimeSpan.FromSeconds(20);

        private readonly DataConnectionFactory _factory;

        public VersioningService(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<long> WaitForVersion(Guid token)
        {
            long? amsVersion;
            if (AmsHelper.TryGetLatestMessage(0, out var amsMessage))
            {
                var utcNow = DateTime.UtcNow;
                var amsUtcNow = amsMessage.Timestamp.UtcDateTime;
                if ((utcNow - amsUtcNow).Duration() > AmsSyncInterval)
                {
                    throw new TimeoutException($"River clock {utcNow} and Ams clock {amsUtcNow} differs more than {AmsSyncInterval}");
                }

                amsVersion = await WaitForAmsState(amsMessage.Offset, AmsWaitAttempts, WaitInterval);
                if (amsVersion == null)
                {
                    throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for AMS state failed after {0} attempts", AmsWaitAttempts));
                }
            }
            else
            {
                amsVersion = 0;
            }

            var ermVersion = await WaitForErmState(token, ErmWaitAttempts, WaitInterval);
            if (ermVersion == null)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for ERM state failed after {0} attempts", ErmWaitAttempts));
            }

            return ermVersion.Value > amsVersion.Value ? ermVersion.Value : amsVersion.Value;
        }

        private async Task<long?> WaitForAmsState(long offset, int waitAttempts, TimeSpan waitInterval)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var connectionLocal = connection;

                var version = await Waiter(async () =>
                {
                    var amsState = await connectionLocal.GetTable<Version.AmsState>().SingleOrDefaultAsync(x => x.Offset == offset);
                    return amsState?.VersionId;
                }, waitAttempts, waitInterval);

                return version;
            }
        }

        private async Task<long?> WaitForErmState(Guid token, int waitAttempts, TimeSpan waitInterval)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var connectionLocal = connection;

                var version = await Waiter(async () =>
                {
                    var ermState = await connectionLocal.GetTable<Version.ErmState>().SingleOrDefaultAsync(x => x.Token == token);
                    return ermState?.VersionId;
                }, waitAttempts, waitInterval);

                return version;
            }
        }

        private static async Task<T?> Waiter<T>(Func<Task<T?>> func, int waitAttempts, TimeSpan waitInterval) where T: struct
        {
            if (waitAttempts == 0)
            {
                return null;
            }

            var result = await func();
            if (result != null)
            {
                return result;
            }

            var counter = 1;
            while (counter < waitAttempts)
            {
                await Task.Delay(waitInterval);
                result = await func();
                if (result != null)
                {
                    return result;
                }

                counter++;
            }

            return null;
        }

        public long GetLatestVersion()
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                return connection.GetTable<Version.ValidationResult>().Max(x => x.VersionId);
            }
        }

        private static class AmsHelper
        {
            private static readonly AmsSettings Settings = new AmsSettings();

            public static bool TryGetLatestMessage(int partition, out Message message)
            {
                using (var consumer = new Consumer(Settings.Config))
                {
                    var topicPartition = new TopicPartition(Settings.Topic, partition);

                    var offsets = consumer.QueryWatermarkOffsets(topicPartition, Settings.Timeout);
                    consumer.Assign(new[] { new TopicPartitionOffset(topicPartition, offsets.High - 1) });

                    return consumer.Consume(out message, Settings.Timeout);
                }
            }

            private sealed class AmsSettings
            {
                private const string ClientId = "ValidationRules.Querying.Host";

                public AmsSettings()
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["Ams"].ConnectionString;
                    Config = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);

                    var environmentName = ConfigurationManager.AppSettings["TargetEnvironmentName"];

                    Config.Add("client.id", ClientId + '-' + environmentName);
                    Config.Add("group.id", ClientId + '-' + environmentName);

                    Topic = ConfigurationManager.AppSettings["AmsFactsTopics"].Split(',').First();
                }

                public Dictionary<string, object> Config { get; }
                public string Topic { get; }
                public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
            }
        }
    }
}