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
        private const int WaitAttempts = 60;
        private static readonly TimeSpan WaitInterval = TimeSpan.FromSeconds(5);

        private readonly DataConnectionFactory _factory;

        public VersioningService(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<long> WaitForVersion(Guid token)
        {
            var offset = AmsHelper.GetLatestOffset();

            var ermVersion = await WaitForErmState(token, WaitAttempts, WaitInterval);
            if (ermVersion == null)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for ERM state failed after {0} attempts", WaitAttempts));
            }

            var amsVersion = await WaitForAmsState(offset, WaitAttempts, WaitInterval);
            if (amsVersion == null)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for AMS state failed after {0} attempts", WaitAttempts));
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

            public static long GetLatestOffset(int partition = 0)
            {
                using (var consumer = new Consumer(Settings.Config))
                {
                    var offsets = consumer.QueryWatermarkOffsets(new TopicPartition(Settings.Topic, partition), Settings.QueryOffsetsTimeout);
                    return offsets.High;
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

                    var topic = ConfigurationManager.AppSettings["AmsFactsTopics"];
                    if (topic != null)
                    {
                        Topic = topic.Split(',').First();
                    }
                }

                public Dictionary<string, object> Config { get; }
                public string Topic { get; } = "ams_okapi_vr_integration.am.validity";
                public TimeSpan QueryOffsetsTimeout { get; } = TimeSpan.FromSeconds(5);
            }
        }
    }
}