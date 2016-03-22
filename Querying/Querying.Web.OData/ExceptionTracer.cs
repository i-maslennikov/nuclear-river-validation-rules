using System.Web.Http.ExceptionHandling;

using NuClear.Tracing.API;

namespace NuClear.Querying.Web.OData
{
    public sealed class ExceptionTracer : ExceptionLogger
    {
        private readonly ITracer _tracer;

        public ExceptionTracer(ITracer tracer)
        {
            _tracer = tracer;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            _tracer.ErrorFormat(context.Exception, "Unhandled exception processing {0} for {1}", context.Request.Method, context.Request.RequestUri);
        }
    }
}