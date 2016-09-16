using System;
using System.Linq;
using System.Reflection;

using LinqToDB.Linq;

using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.Replication
{
    public sealed class QueryTracer
    {
        private readonly ITelemetryPublisher _publisher;
        private readonly ITracer _tracer;

        public QueryTracer(ITelemetryPublisher publisher, ITracer tracer)
        {
            _publisher = publisher;
            _tracer = tracer;
        }

        public void Trace<T>(IQueryable<T> queryable)
        {
            try
            {
                var queryId = Guid.NewGuid();
                Probe.Create($"Trace query {queryId}").Dispose();

                var innerQueryable = (IQueryable<T>)queryable.GetType().GetField("_innerQueryable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(queryable);
                var sqlText = ((IExpressionQuery<T>)innerQueryable).SqlText;
                _publisher.Trace($"Trace query {queryId}", sqlText);
            }
            catch (Exception exception)
            {
                _tracer.Error(exception, "No query information will be provided");
            }
        }
    }
}