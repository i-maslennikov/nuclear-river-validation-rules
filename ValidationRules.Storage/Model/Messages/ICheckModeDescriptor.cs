using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public interface ICheckModeDescriptor
    {
        CheckMode CheckMode { get; }

        IReadOnlyDictionary<MessageTypeCode, RuleSeverityLevel> Rules { get; }

        // Костыль для проверки заказа "на расторжении" только в той его части, которая ещё не размещалась
        // https://github.com/2gis/nuclear-river-validation-rules/issues/193
        DateTime GetValidationPeriodStart(Order order);
    }
}