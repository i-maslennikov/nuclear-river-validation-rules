using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreReceiver : MessageReceiverBase<PerformedOperationsFinalProcessingMessage, IPerformedOperationsReceiverSettings>
    {
        private readonly IQuery _query;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;

        public SqlStoreReceiver(
            MessageFlowMetadata sourceFlowMetadata,
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IQuery query,
            IRepository<PerformedOperationFinalProcessing> repository)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _query = query;
            _repository = repository;
        }

        protected override IReadOnlyList<PerformedOperationsFinalProcessingMessage> Peek()
        {
            var messages = _query.For<PerformedOperationFinalProcessing>()
                                 .Where(processing => processing.MessageFlowId == SourceFlowMetadata.MessageFlow.Id)
                                 .Take(MessageReceiverSettings.BatchSize)
                                 .ToArray();

            return messages.Select(x => new PerformedOperationsFinalProcessingMessage
                {
                    Flow = SourceFlowMetadata.MessageFlow,
                    FinalProcessings = new[] { x },
                }).ToArray();
        }

        protected override void Complete(
            IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages,
            IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            // COMMENT {all, 05.05.2015}: Что делать при ошибках во время обработки?
            // Сейчас и на стадии Primary и на стадии Final сообщение будет пытаться обработаться до тех пор, пока не получится.
            // Или пока админ не удалит его из очереди.

            // We need to use different transaction scope to operate with operation receiver because it has its own store
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _repository.DeleteRange(successfullyProcessedMessages.SelectMany(message => message.FinalProcessings));
                _repository.Save();

                transaction.Complete();
            }
        }
    }
}
