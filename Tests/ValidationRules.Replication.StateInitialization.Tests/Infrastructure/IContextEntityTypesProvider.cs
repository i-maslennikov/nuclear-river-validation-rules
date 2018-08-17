using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure
{
    public interface IContextEntityTypesProvider
    {
        IReadOnlyCollection<Type> GetTypesFromContext(string context);
    }
}