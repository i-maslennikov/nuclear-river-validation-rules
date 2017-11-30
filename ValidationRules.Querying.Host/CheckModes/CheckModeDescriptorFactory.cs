using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Erm;
using NuClear.ValidationRules.Storage.Model.Messages;
using System.Linq;

namespace NuClear.ValidationRules.Querying.Host.CheckModes
{
    public class CheckModeDescriptorFactory
    {
        private static readonly IReadOnlyDictionary<CheckMode, Dictionary<MessageTypeCode, RuleSeverityLevel>> CheckModes =
            CheckModeRegistry.Map.SelectMany(x => x.Item2.Select(y => new { MessageTypeCode = x.Item1, CheckMode = y.Key, RuleSeverityLevel = y.Value }))
                         .GroupBy(x => x.CheckMode)
                         .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.MessageTypeCode, y => y.RuleSeverityLevel));

        public ICheckModeDescriptor GetDescriptorFor(CheckMode checkMode)
        {
            Dictionary<MessageTypeCode, RuleSeverityLevel> checkModeRules;
            if (!CheckModes.TryGetValue(checkMode, out checkModeRules))
            {
                throw new ArgumentException($"Check mode {checkMode} nas no one rule", nameof(checkModeRules));
            }

            return new CheckModeDescriptor(checkMode, checkModeRules);
        }

        private sealed class CheckModeDescriptor : ICheckModeDescriptor
        {
            private readonly CheckMode _checkMode;
            private readonly IReadOnlyDictionary<MessageTypeCode, RuleSeverityLevel> _checkModeRules;
            private readonly HashSet<MessageTypeCode> _rules;

            public CheckModeDescriptor(CheckMode checkMode, IReadOnlyDictionary<MessageTypeCode, RuleSeverityLevel> checkModeRules)
            {
                _checkMode = checkMode;
                _checkModeRules = checkModeRules;
                _rules = new HashSet<MessageTypeCode>(checkModeRules.Keys);
            }

            public IReadOnlyCollection<MessageTypeCode> Rules => _rules;

            public RuleSeverityLevel GetRuleSeverityLevel(MessageTypeCode rule)
            {
                RuleSeverityLevel level;
                if (!_checkModeRules.TryGetValue(rule, out level))
                {
                    throw new ArgumentException($"Rule {rule} is not defined for check mode {_checkMode}", nameof(rule));
                }

                return level;
            }

            public DateTime GetValidationPeriodStart(Order order)
            {
                const int OrderStateOnTermination = 4;
                if (_checkMode == CheckMode.SingleForApprove && order.WorkflowStepId == OrderStateOnTermination)
                {
                    return order.EndDistributionDateFact.AddSeconds(1);
                }

                return order.BeginDistributionDate;
            }
        }
    }
}
