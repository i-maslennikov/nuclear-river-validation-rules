using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public class IndirectlyDependentAggregateFeature<T, TKey> : IIndirectFactDependencyFeature, IFactDependencyFeature<T, TKey>
        where T : IIdentifiable<TKey>
    {
        public IndirectlyDependentAggregateFeature(IIdentityProvider<TKey> identityProvider, MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider)
        {
            FindSpecificationProvider = keys => new FindSpecification<T>(identityProvider.Create<T, TKey>(keys));

            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                  = MapSpecificationProviderOnDelete
                    = mapSpecificationProvider;
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