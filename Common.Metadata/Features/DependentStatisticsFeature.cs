using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    // TODO {all, 15.09.2015}: Подумать о правильном поядке вызова при создании/обновлении/удалении факта (до/после - аналогично *DependentAggregateFeature или должен отличаться?)
    public class DependentStatisticsFeature<T> : IIndirectFactDependencyFeature, IFactDependencyFeature<T> where T : IIdentifiable<DefaultIdentity, long>
    {
        public DependentStatisticsFeature(MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider)
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