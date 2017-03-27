using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NuClear.Telemetry;
using NuClear.Telemetry.Logstash;
using NuClear.Telemetry.Probing;

namespace NuClear.ValidationRules.Querying.Host
{
    public sealed class TelemetryMessageHandler : DelegatingHandler
    {
        private readonly ITelemetryPublisher _telemetry;

        public TelemetryMessageHandler()
        {
            _telemetry = new LogstashTelemetryPublisher(new LogstashSettingsAspect());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            foreach (var report in DefaultReportSink.Instance.ConsumeReports())
            {
                _telemetry.Trace("ProbeReport", report);
            }

            return response;
        }
    }
}