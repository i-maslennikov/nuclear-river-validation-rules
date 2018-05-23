using System;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MessageSeverityProvider : IMessageSeverityProvider
    {
        public RuleSeverityLevel GetLevel(Message message, ICheckModeDescriptor checkModeDescriptor)
        {
            if (message.MessageType != MessageTypeCode.LinkedFirmAddressShouldBeValid)
            {
                return GetConfiguredLevel();
            }

            var isPartnerAddress = bool.Parse(message.Extra["isPartnerAddress"]);
            return isPartnerAddress ? RuleSeverityLevel.Warning : GetConfiguredLevel();

            RuleSeverityLevel GetConfiguredLevel()
            {
                if (!checkModeDescriptor.Rules.TryGetValue(message.MessageType, out var level))
                {
                    throw new ArgumentException($"Could not find level for message '{message.MessageType}'");
                }

                return level;
            }
        }
    }
}