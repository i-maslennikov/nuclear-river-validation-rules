using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    public sealed class CachingTelemetryPublisherDecorator<TPublisher> : ITelemetryPublisher, IDisposable
        where TPublisher : ITelemetryPublisher
    {
        private readonly ITracer _tracer;
        private readonly TPublisher _publisher;
        private readonly ConcurrentDictionary<Action<long>, long> _dictionary;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _flusher;

        public CachingTelemetryPublisherDecorator(TPublisher publisher, ITracer tracer)
        {
            _publisher = publisher;
            _tracer = tracer;
            _cancellationTokenSource = new CancellationTokenSource();
            _dictionary = new ConcurrentDictionary<Action<long>, long>();
            _flusher = Task.Factory.StartNew(() => Flush(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        public void Publish<T>(long value)
            where T : TelemetryIdentityBase<T>, new()
        {
            _dictionary.AddOrUpdate(_publisher.Publish<T>, value, (action, accumulated) => accumulated + value);
        }

        public void Trace(string message, object data)
        {
            _publisher.Trace(message, data);
        }

        private void Flush(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    foreach (var key in _dictionary.Keys)
                    {
                        long value;
                        if (_dictionary.TryRemove(key, out value))
                        {
                            key.Invoke(value);
                        }
                    }

                    Task.Delay(500, token).Wait(token);
                }
                catch (Exception exception)
                {
                    _tracer.Error(exception, "Ошибка при асинхронной отправке телеметрии");
                    Task.Delay(5000, token).Wait(token);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel(false);

            try
            {
                _flusher.Wait();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Ошибка завершения асинхронной отправке телеметрии");
            }
        }
    }
}
