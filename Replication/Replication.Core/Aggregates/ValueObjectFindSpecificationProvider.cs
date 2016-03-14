using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectFindSpecificationProvider<T> : IFindSpecificationProvider<T, AggregateOperation>
    {
        private readonly ValueObjectMetadata<T, long> _metadata;

        public ValueObjectFindSpecificationProvider(ValueObjectMetadata<T, long> metadata)
        {
            _metadata = metadata;
        }

        public FindSpecification<T> Create(IEnumerable<AggregateOperation> commands)
        {
            return _metadata.FindSpecificationProvider.Invoke(commands.Select(c => c.EntityId).ToArray());
        }
    }
}