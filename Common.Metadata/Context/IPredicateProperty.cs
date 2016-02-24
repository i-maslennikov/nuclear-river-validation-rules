using System.Collections.Generic;

namespace NuClear.River.Common.Metadata.Context
{
    public interface IPredicateProperty<T>
    {
        T GetValue(Predicate p);
        void SetValue(IDictionary<string, string> properties, T value);
    }
}