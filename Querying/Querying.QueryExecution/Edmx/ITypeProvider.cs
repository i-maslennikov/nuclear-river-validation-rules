using System;

using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.Edm.Edmx
{
    public interface ITypeProvider
    {
        Type Resolve(EntityElement entityElement);
    }
}