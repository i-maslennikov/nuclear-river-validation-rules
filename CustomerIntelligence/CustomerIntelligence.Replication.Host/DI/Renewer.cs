using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.Replication.Host.DI
{
    public sealed class TunedReceivedMessagesLockRenewalManager : IServiceBusLockRenewer
    {
        private readonly ITracer _tracer;
        private readonly ConcurrentDictionary<Guid, BrokeredMessage> _attachedMessages = new ConcurrentDictionary<Guid, BrokeredMessage>();
        private readonly BlockingCollection<IReadOnlyCollection<BrokeredMessage>> _renewalQueue = new BlockingCollection<IReadOnlyCollection<BrokeredMessage>>();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Task _renewalManager;
        private readonly Task _renewer;

        public TunedReceivedMessagesLockRenewalManager(ITracer tracer, IServiceBusMessageLockRenewalSettings settings)
        {
            var checkInterval = TimeSpan.FromSeconds(settings.LockRenewalInterval);
            var timeReserve = TimeSpan.FromSeconds(settings.LockRenewalTimeReserve);

            _tracer = tracer;
            _renewalManager = RenewalManager(checkInterval, timeReserve, _cancellationTokenSource.Token);
            _renewer = Task.Factory.StartNew(() => Renewer(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Attach(IEnumerable<BrokeredMessage> messages)
        {
            foreach (var message in messages)
            {
                _attachedMessages.TryAdd(message.LockToken, message);
            }
        }

        public void Detach(IEnumerable<Guid> lockIds)
        {
            foreach (var lockId in lockIds)
            {
                BrokeredMessage detachedMessage;
                _attachedMessages.TryRemove(lockId, out detachedMessage);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel(false);
            _attachedMessages.Clear();
            _renewalQueue.CompleteAdding();

            try
            {
                _renewalManager.Wait();
                _renewer.Wait();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Renewal producer/consumer finished with failures");
            }
        }

        private async Task RenewalManager(TimeSpan checkInterval, TimeSpan timeReserve, CancellationToken cancellationToken)
        {
            _tracer.Debug("RenewalManager started");
            var ttlMinimum = checkInterval + timeReserve;
            while (!cancellationToken.IsCancellationRequested)
            {
                _tracer.Debug("RenewalManager loop");
                var currentTime = DateTime.UtcNow;
                var batch = new List<BrokeredMessage>();
                foreach (var message in _attachedMessages.Values)
                {
                    var ttl = message.LockedUntilUtc - currentTime;
                    if (ttl <= TimeSpan.Zero)
                    {
                        BrokeredMessage detachedMessage;
                        _tracer.ErrorFormat($"Can not schedule message {message.MessageId} renewal: lock expired, lock: {message.LockToken}");
                        _attachedMessages.TryRemove(message.LockToken, out detachedMessage);
                    }

                    if (ttl <= ttlMinimum)
                    {
                        batch.Add(message);
                    }
                }

                if (batch.Any())
                {
                    _renewalQueue.TryAdd(batch);
                }

                await Task.Delay(checkInterval, cancellationToken);
            }
        }

        private void Renewer(CancellationToken cancellationToken)
        {
            _tracer.Debug("Renewer started");
            var enumerable = _renewalQueue.GetConsumingEnumerable(cancellationToken);
            foreach (var renewingBatch in enumerable)
            {
                _tracer.Info($"Renewing {renewingBatch.Count} messages");
                Parallel.ForEach(renewingBatch, RenewMessage);
                _tracer.Debug("Renewing complete");
            }
        }

        private void RenewMessage(BrokeredMessage message)
        {
            try
            {
                message.RenewLock();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, $"Can't renew message {message.MessageId} with lock id {message.LockToken}");
            }
        }
    }

    static class Extensions
    {
        public static object GetPropertyValue(this object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return prop.GetValue(obj);
        }

        public static object ReflectionInvoke(this object obj, string methodName, params object[] objects)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return method.Invoke(obj, objects);
        }
    }
}