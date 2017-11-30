using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public interface ICheckModeDescriptor
    {
        IReadOnlyCollection<MessageTypeCode> Rules { get; }
        RuleSeverityLevel GetRuleSeverityLevel(MessageTypeCode rule);

        // Костыль для проверки заказа "на расторжении" только в той его части, которая ещё не размещалась
        // https://github.com/2gis/nuclear-river-validation-rules/issues/193
        DateTime GetValidationPeriodStart(Order order);
    }
}