using System.Collections.Generic;
using System.Reflection;

namespace NuClear.Replication.Core.API.Equality
{
    public interface IObjectPropertyProvider
    {
        IReadOnlyCollection<PropertyInfo> GetPrimaryKeyProperties<T>();
        IReadOnlyCollection<PropertyInfo> GetProperties<T>();
    }
}