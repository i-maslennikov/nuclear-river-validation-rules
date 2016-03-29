using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public interface IChildEntityProcessor<TRootEntity>
    {
        Type ChildEntityType { get; }
        void Initialize(FindSpecification<TRootEntity> specification);
        void Recalculate(FindSpecification<TRootEntity> specification);
        void Destroy(FindSpecification<TRootEntity> specification);
        void RecalculatePartially(FindSpecification<TRootEntity> specification, IReadOnlyCollection<RecalculateAggregatePart> commands);
    }
}