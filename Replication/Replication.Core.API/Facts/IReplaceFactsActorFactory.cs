using System;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IReplaceFactsActorFactory
    {
        IActor Create(Type commandType);
    }
}