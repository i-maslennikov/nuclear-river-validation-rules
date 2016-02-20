using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public class IndirectlyDependentAggregateFeature<T> : IIndirectFactDependencyFeature, IFactDependencyFeature<T>
        where T : IIdentifiable<long>
    {
        public IndirectlyDependentAggregateFeature(MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider)
        {
            FindSpecificationProvider = Specs.Find.ByIds<T>;

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
        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; private set; }
    }
}