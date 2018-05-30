using System;
using System.Collections.Generic;
using System.Text;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<RulesetFactsFlow, KafkaMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory<KafkaMessage> _commandFactory;

        public RulesetFactsFlowAccumulator()
        {
            _commandFactory = new RulesetFactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(KafkaMessage kafkaMessage)
        {
            return new AggregatableMessage<ICommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = _commandFactory.CreateCommands(kafkaMessage),
                };
        }

        private sealed class RulesetFactsCommandFactory : ICommandFactory<KafkaMessage>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(KafkaMessage kafkaMessage)
            {
                // filter tombstone messages
                var kafkaMessagePayload = kafkaMessage.Message.Value;
                if (kafkaMessagePayload == null)
                {
                    return Array.Empty<ICommand>();
                }

                var rawXmlRulesetMessage = Encoding.UTF8.GetString(kafkaMessagePayload);
                var rulesetDto = ConvertToRulesetDto(rawXmlRulesetMessage);

                return new[]
                    {
                        new ReplaceDataObjectCommand(typeof(Ruleset),
                                                     new [] { rulesetDto }),
                        new ReplaceDataObjectCommand(typeof(Ruleset.AssociatedRule),
                                                     new [] { rulesetDto }),
                        new ReplaceDataObjectCommand(typeof(Ruleset.DeniedRule),
                                                     new [] { rulesetDto }),
                        new ReplaceDataObjectCommand(typeof(Ruleset.QuantitativeRule),
                                                     new [] { rulesetDto }),
                        new ReplaceDataObjectCommand(typeof(Ruleset.RulesetProject),
                                                     new [] { rulesetDto })
                    };
            }

            private static RulesetDto ConvertToRulesetDto(string rawXmlRulesetMessage)
            {
                return new RulesetDto();
            }
        }
    }
}