using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectFindSpecificationProvider<TValueObject, TEntity, TEntityKey> : IFindSpecificationProvider<TValueObject, TEntity>
        where TEntity : IIdentifiable<TEntityKey>
    {
        private readonly ValueObjectMetadata<TValueObject, TEntityKey> _metadata;
        private readonly Func<TEntity, TEntityKey> _entityIdentityProvider;

        public ValueObjectFindSpecificationProvider(ValueObjectMetadata<TValueObject, TEntityKey> metadata, IIdentityProvider<TEntityKey> entityIdentityProvider)
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