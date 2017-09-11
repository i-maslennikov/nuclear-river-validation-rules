using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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

        private static readonly TimeSpan WaitTimeout = TimeSpan.FromMinutes(6);
        private static readonly TimeSpan WaitInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan AmsSyncInterval = TimeSpan.FromSeconds(20);

        private readonly DataConnectionFactory _factory;

        public VersioningService(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<long> WaitForVersion(Guid token)
        {
            Task<long> amsVersion;
            if (AmsHelper.TryGetLatestMessage(0, out var amsMessage))
            {
                var utcNow = DateTime.UtcNow;
                var amsUtcNow = amsMessage.Timestamp.UtcDateTime;
                if ((utcNow - amsUtcNow).Duration() > AmsSyncInterval)
                {
                    throw new TimeoutException($"River clock {utcNow} and Ams clock {amsUtcNow} (offset {amsMessage.Offset}) differs more than {AmsSyncInterval}");
                }

                amsVersion = WaitForAmsState(amsMessage.Offset, WaitInterval, WaitTimeout);
            }
            else
            {
                amsVersion = Task.FromResult(0L);
            }

            var ermVersion = WaitForErmState(token, WaitInterval, WaitTimeout);

            var result = await Task.WhenAll(amsVersion, ermVersion);

            return result.Max();
        }

        private async Task<long> WaitForAmsState(long offset, TimeSpan interval, TimeSpan timeout)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var connectionLocal = connection;

                var version = await Waiter(async () =>
                {
                    var amsState = await connectionLocal.GetTable<Version.AmsState>().SingleOrDefaultAsync(x => x.Offset >= offset);
                    return amsState?.VersionId;
                }, interval, timeout);


                if (version == null)
                {
                    throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for AMS state {0} failed after {1}", offset, timeout));
                }

                return version.Value;
            }
        }

        private async Task<long> WaitForErmState(Guid token, TimeSpan interval, TimeSpan timeout)
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var connectionLocal = connection;

                var version = await Waiter(async () =>
                {
                    var ermState = await connectionLocal.GetTable<Version.ErmState>().SingleOrDefaultAsync(x => x.Token == token);
                    return ermState?.VersionId;
                }, interval, timeout);

                if (version == null)
                {
                    throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for ERM state {0} failed after {1}", token, timeout));
                }

                return version.Value;
            }
        }

        private static async Task<T?> Waiter<T>(Func<Task<T?>> func, TimeSpan interval, TimeSpan timeout) where T : struct
        {
            var attemptCount = timeout.Ticks / interval.Ticks;
            for (var i = 0; i < attemptCount; i++)
            {
                var sw = Stopwatch.StartNew();
                var result = await func();
                if (result != null)
                {
                    return result;
                }

                sw.Stop();
                await Task.Delay(sw.Elapsed < interval ? interval - sw.Elapsed : TimeSpan.Zero);
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
                public AmsSettings()
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["Ams"].ConnectionString;
                    Config = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
                    Config.Add("group.id", Guid.NewGuid().ToString());

                    Topic = ConfigurationManager.AppSettings["AmsFactsTopics"].Split(',').First();
                }

                public Dictionary<string, object> Config { get; }
                public string Topic { get; }
                public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
            }
        }
    }
}