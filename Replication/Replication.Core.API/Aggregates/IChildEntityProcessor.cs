using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IChildEntityProcessor<TParentEntityKey>
    {
        Type EntityType { get; }
        void Initialize(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
        void Recalculate(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
        void Recalculate(IReadOnlyCollection<IGrouping<TParentEntityKey, object>> entityKeys);
        void Destroy(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
    }
}