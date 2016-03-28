using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectFindSpecificationProvider<TValueObject, TEntity> : IFindSpecificationProvider<TValueObject, TEntity>
        where TEntity : IIdentifiable<long>
    {
        private readonly ValueObjectMetadata<TValueObject, long> _metadata;
        private readonly Func<TEntity, long> _entityIdentityProvider;

        public ValueObjectFindSpecificationProvider(ValueObjectMetadata<TValueObject, long> metadata, IIdentityProvider<long> entityIdentityProvider)
        {
            _metadata = metadata;
            _entityIdentityProvider = entityIdentityProvider.Get<TEntity>().Compile();
        }

        public FindSpecification<TValueObject> Create(IEnumerable<TEntity> entities)
        {
            return _metadata.FindSpecificationProvider.Invoke(entities.Select(_entityIdentityProvider.Invoke).ToArray());
        }
    }
}