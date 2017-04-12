using System;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public enum RuleSeverityLevel
    {
        [Obsolete]
        None = 0,

        Info = 1,
        Warning = 2,
        Error = 3,
    }
}