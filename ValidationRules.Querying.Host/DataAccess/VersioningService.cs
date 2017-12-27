using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB;

using NuClear.Messaging.API.Flows;

using ValidationRules.Hosting.Common;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class VersioningService
    {
        private const string ConfigurationString = "Messages";

        private static readonly TimeSpan WaitTimeout = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan WaitInterval = TimeSpan.FromSeconds(5);

        private readonly DataConnectionFactory _factory;
        private readonly KafkaMessageFlowInfoProvider _kafkaMessageFlowInfoProvider;

        public VersioningService(DataConnectionFactory factory, KafkaMessageFlowInfoProvider kafkaMessageFlowInfoProvider)
        {
            _factory = factory;
            _kafkaMessageFlowInfoProvider = kafkaMessageFlowInfoProvider;
        }

        public async Task<long> WaitForVersion(Guid token)
        {
            var flowSize = _kafkaMessageFlowInfoProvider.GetFlowSize(AmsFactsFlow.Instance);
            var amsVersion = flowSize != 0 ? WaitForAmsState(flowSize - 1, WaitInterval, WaitTimeout) : Task.FromResult(0L);
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
                    var amsState = await connectionLocal.GetTable<Version.AmsState>().OrderBy(x => x.VersionId).FirstOrDefaultAsync(x => x.Offset >= offset);
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

        // не хочется референсить на OperationProcessing, поэтому копипаст
        private sealed class AmsFactsFlow : MessageFlowBase<AmsFactsFlow>
        {
            public override Guid Id => new Guid("A2878E80-992A-4602-8FD6-B10AE85BBFFE");

            public override string Description => nameof(AmsFactsFlow);
        }
    }
}