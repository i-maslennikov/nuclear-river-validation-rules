using System;

using Confluent.Kafka;

using NuClear.Jobs;
using NuClear.OperationsLogging.API;
using NuClear.Replication.Core;
using NuClear.Security.API.Auth;
using NuClear.Security.API.Context;
using NuClear.Storage.API.Writings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.Replication.Events;

using Quartz;

using ValidationRules.Hosting.Common;

using SystemStatus = NuClear.ValidationRules.Storage.Model.Facts.SystemStatus;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    public sealed class HeartbeatJob : TaskServiceJobBase
    {
        private static readonly TimeSpan AmsSyncInterval = TimeSpan.FromSeconds(20);

        private readonly IRepository<SystemStatus> _repository;
        private readonly IEventLogger _eventLogger;

        public HeartbeatJob(IUserContextManager userContextManager,
                            IUserAuthenticationService userAuthenticationService,
                            IUserAuthorizationService userAuthorizationService,
                            ITracer tracer,
                            IRepository<SystemStatus> repository,
                            IEventLogger eventLogger)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, tracer)
        {
            _repository = repository;
            _eventLogger = eventLogger;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            SendAmsHeartbeat();
        }

        private void SendAmsHeartbeat()
        {
            using (var consumer = new Consumer(ConsumerExtensions.Settings.Config))
            {
                if (consumer.TryGetLatestMessage(0, out var amsMessage))
                {
                    var utcNow = DateTime.UtcNow;
                    var amsUtcNow = amsMessage.Timestamp.UtcDateTime;
                    var amsIsDown = (utcNow - amsUtcNow).Duration() > AmsSyncInterval;

                    _repository.Update(new SystemStatus { Id = SystemStatus.SystemId.Ams, SystemIsDown = amsIsDown });
                    _repository.Save();

                    _eventLogger.Log<IEvent>(new[] { new FlowEvent(AmsFactsFlow.Instance, new DataObjectUpdatedEvent(typeof(SystemStatus), SystemStatus.SystemId.Ams)) });
                }
            }
        }
    }
}