using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.OperationsLogging.API;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public sealed class EventLoggingStrategyDecorator<TEvent> : IEventLoggingStrategy<TEvent>
    {
        private readonly IEventLoggingStrategy<TEvent> _strategy;
        private readonly Func<TEvent, bool> _filter;
        private readonly Action<long> _reportMessageCount;

        public EventLoggingStrategyDecorator(IEventLoggingStrategy<TEvent> strategy, Func<TEvent, bool> filter, Action<long> reportMessageCount)
        {
            _strategy = strategy;
            _filter = filter;
            _reportMessageCount = reportMessageCount;
        }

        public bool TryLog(IReadOnlyCollection<TEvent> events, out string report)
        {
            using (Probe.Create("Send events"))
            {
                var flowEvents = events.Where(_filter).ToArray();
                _reportMessageCount.Invoke(flowEvents.Length);
                return _strategy.TryLog(flowEvents, out report);
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