using System;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MessageSeverityProvider : IMessageSeverityProvider
    {
        public RuleSeverityLevel GetLevel(Message message, ICheckModeDescriptor checkModeDescriptor)
        {
            switch (message.MessageType)
            {
                case MessageTypeCode.OrderRequiredFieldsShouldBeSpecified:
                    //Понижаем уровень ошибки LegalPersonProfile до Warning для не Single проверок
                    var isLegalPersonProfile = bool.Parse(message.Extra["legalPersonProfile"]);
                    var isCurrency = bool.Parse(message.Extra["currency"]);
                    var isBranchOfficeOrganizationUnit = bool.Parse(message.Extra["branchOfficeOrganizationUnit"]);
                    var isLegalPerson = bool.Parse(message.Extra["legalPerson"]);

                    return checkModeDescriptor.CheckMode != CheckMode.Single
                           && !isCurrency && !isBranchOfficeOrganizationUnit && !isLegalPerson && isLegalPersonProfile
                               ? RuleSeverityLevel.Warning
                               : GetConfiguredLevel(message, checkModeDescriptor);

                case MessageTypeCode.LinkedFirmAddressShouldBeValid:
                    var isPartnerAddress = bool.Parse(message.Extra["isPartnerAddress"]);
                    return isPartnerAddress
                               ? RuleSeverityLevel.Warning
                               : GetConfiguredLevel(message, checkModeDescriptor);

                default:
                    return GetConfiguredLevel(message, checkModeDescriptor);
            }
        }

        private RuleSeverityLevel GetConfiguredLevel(Message message, ICheckModeDescriptor checkModeDescriptor)
        {
            if (!checkModeDescriptor.Rules.TryGetValue(message.MessageType, out var level))
            {
                throw new ArgumentException($"Could not find level for message '{message.MessageType}'");
            }

            return level;
        }
    }
}