using System.Collections.Generic;
using System.Linq;

using NuClear.OperationsLogging.API;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public class EventLoggingStrategyDecorator<TEvent> : IEventLoggingStrategy<TEvent>
    {
        private readonly IEventLoggingStrategy<TEvent> _strategy;
        private readonly IFlowAspect<TEvent> _flowAspect;

        public EventLoggingStrategyDecorator(IEventLoggingStrategy<TEvent> strategy, IFlowAspect<TEvent> flowAspect)
        {
            _strategy = strategy;
            _flowAspect = flowAspect;
        }

        public bool TryLog(IReadOnlyCollection<TEvent> events, out string report)
        {
            using (Probe.Create("Log events"))
            {
                var flowEvents = events.Where(_flowAspect.ShouldEventBeLogged).ToArray();
                if (_strategy.TryLog(flowEvents, out report))
                {
                    _flowAspect.ReportMessageLoggedCount(flowEvents.Length);
                    return true;
                }

                return false;
            }
        }

        public ILoggingSession Begin()
        {
            return _strategy.Begin();
        }

        public void Complete(ILoggingSession loggingSession)
        {
            _strategy.Complete(loggingSession);
        }

        public void Close(ILoggingSession loggingSession)
        {
            _strategy.Close(loggingSession);
        }
    }
}