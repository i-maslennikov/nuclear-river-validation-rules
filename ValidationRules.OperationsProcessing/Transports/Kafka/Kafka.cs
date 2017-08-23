using System;
using System.Collections.Generic;
using System.Linq;

using Confluent.Kafka;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.Transports.Kafka;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.OperationsProcessing.Transports.Kafka
{
    #region move to NuClear.OperationsProcessing.Transports.Kafka

    public sealed class KafkaMessage : MessageBase
    {
        public KafkaMessage(IReadOnlyCollection<Message> messages)
        {
            Id = Guid.NewGuid();
            Messages = messages;
        }

        public override Guid Id { get; }

        public IReadOnlyCollection<Message> Messages { get; }
    }

    public sealed class KafkaReceiver : MessageReceiverBase<KafkaMessage, IPerformedOperationsReceiverSettings>
    {
        private readonly IKafkaMessageFlowReceiver _messageFlowReceiver;
        private readonly ITracer _tracer;

        public KafkaReceiver(
            MessageFlowMetadata sourceFlowMetadata,
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IKafkaMessageFlowReceiverFactory messageFlowReceiverFactory,
            ITracer tracer)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _tracer = tracer;
            _messageFlowReceiver = messageFlowReceiverFactory.Create(SourceFlowMetadata.MessageFlow);
        }

        protected override IReadOnlyList<KafkaMessage> Peek()
        {
            var messages = _messageFlowReceiver.ReceiveBatch(MessageReceiverSettings.BatchSize);
            return new[] { new KafkaMessage(messages) };
        }

        protected override void Complete(IEnumerable<KafkaMessage> successfullyProcessedMessages, IEnumerable<KafkaMessage> failedProcessedMessages)
        {
            if (failedProcessedMessages.Any())
            {
                _tracer.WarnFormat("Kafka processing stopped, some messages cannot be processed");
                return;
            }

            var lastSuccessfulMessage = successfullyProcessedMessages.SelectMany(x => x.Messages).OrderByDescending(x => x.Offset.Value).FirstOrDefault();
            if (lastSuccessfulMessage != null)
            {
                _messageFlowReceiver.Complete(lastSuccessfulMessage);
            }
        }

    }

    #endregion

    public interface IAmsSettingsFactory
    {
        IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow);
    }

    public sealed class KafkaMessageFlowReceiverFactory : IKafkaMessageFlowReceiverFactory
    {
        private readonly ITracer _tracer;
        private readonly IAmsSettingsFactory _amsSettingsFactory;

        public KafkaMessageFlowReceiverFactory(ITracer tracer, IAmsSettingsFactory amsSettingsFactory)
        {
            _tracer = tracer;
            _amsSettingsFactory = amsSettingsFactory;
        }

        public IKafkaMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            var settings = _amsSettingsFactory.CreateReceiverSettings(messageFlow);
            return new KafkaMessageFlowReceiver(settings, _tracer);
        }
    }
}