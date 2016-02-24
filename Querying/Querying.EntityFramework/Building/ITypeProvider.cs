using System;

using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.EntityFramework.Building
{
    public interface ITypeProvider
    {
        Type Resolve(EntityElement entityElement);
    }
}