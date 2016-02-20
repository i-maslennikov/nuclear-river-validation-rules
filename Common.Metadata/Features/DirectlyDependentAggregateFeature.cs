using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public class DirectlyDependentAggregateFeature<T, TKey> : IFactDependencyFeature<T, TKey>
        where T : class, IIdentifiable<TKey>
    {
        public DirectlyDependentAggregateFeature(
            IIdentityProvider<TKey> identityProvider,
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnCreate,
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnUpdate,
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnDelete)

        {
            MapSpecificationProviderOnCreate = mapSpecificationProviderOnCreate;
            MapSpecificationProviderOnUpdate = mapSpecificationProviderOnUpdate;
            MapSpecificationProviderOnDelete = mapSpecificationProviderOnDelete;
            FindSpecificationProvider = keys => new FindSpecification<T>(identityProvider.Create<T, TKey>(keys));
        }

        public Type DependancyType
        {
            get { return typeof(T); }
        }
        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnCreate { get; private set; }
        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnUpdate { get; private set; }
        public MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnDelete { get; private set; }
        public Func<IReadOnlyCollection<TKey>, FindSpecification<T>> FindSpecificationProvider { get; private set; }
    }
}