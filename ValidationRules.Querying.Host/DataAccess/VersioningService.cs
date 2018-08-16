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

        public long GetLatestVersion()
        {
            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                return connection.GetTable<Version.ValidationResult>().Max(x => x.VersionId);
            }
        }

        public Task<long> WaitForVersion(Guid ermToken)
        {
            var amsCount = _kafkaMessageFlowInfoProvider.GetFlowSize(AmsFactsFlow.Instance);

            return WaitForVersion(ermToken, amsCount, WaitInterval, WaitTimeout);
        }

        private async Task<long> WaitForVersion(Guid ermToken, long amsCount, TimeSpan interval, TimeSpan timeout)
        {
            var ermVersion = (long?)null;
            var amsVersion = (long?)null;

            // don't wait for ams if kafka is empty
            if (amsCount == 0)
            {
                amsVersion = 0;
            }
            var amsOffset = amsCount - 1;

            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var connectionLocal = connection;

                await Waiter(async () =>
                {
                    if (ermVersion == null)
                    {
                        ermVersion = (await connectionLocal.GetTable<Version.ErmState>().SingleOrDefaultAsync(x => x.Token == ermToken))?.VersionId;
                    }

                    if (amsVersion == null)
                    {
                        amsVersion = (await connectionLocal.GetTable<Version.AmsState>().OrderBy(x => x.VersionId).FirstOrDefaultAsync(x => x.Offset >= amsOffset))?.VersionId;
                    }

                    if (ermVersion != null && amsVersion != null)
                    {
                        return true;
                    }

                    return false;
                }, interval, timeout);
            }

            if (ermVersion == null)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for ERM state {0} failed after {1}", ermToken, timeout));
            }

            if (amsVersion == null)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Wait for AMS state {0} failed after {1}", amsOffset, timeout));
            }

            return Math.Max(ermVersion.Value, amsVersion.Value);
        }

        private static async Task Waiter(Func<Task<bool>> func, TimeSpan interval, TimeSpan timeout)
        {
            var attemptCount = timeout.Ticks / interval.Ticks;
            for (var i = 0; i < attemptCount; i++)
            {
                var sw = Stopwatch.StartNew();
                var result = await func();
                if (result)
                {
                    return;
                }

                sw.Stop();
                await Task.Delay(sw.Elapsed < interval ? interval - sw.Elapsed : TimeSpan.Zero);
            }
        }

        // не хочется референсить на OperationProcessing, поэтому копипаст
        internal sealed class AmsFactsFlow : MessageFlowBase<AmsFactsFlow>
        {
            public override Guid Id => new Guid("A2878E80-992A-4602-8FD6-B10AE85BBFFE");

            public override string Description => nameof(AmsFactsFlow);
        }
    }
}