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
        private readonly ITracer _tracer;

        public QueryTracer(ITracer tracer)
        {
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
                _tracer.Info($"Trace query {queryId}\n{sqlText}");
            }
            catch (Exception exception)
            {
                _tracer.Error(exception, "No query information will be provided");
            }
        }
    }
}