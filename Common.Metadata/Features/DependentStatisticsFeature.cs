using System;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    // TODO {all, 15.09.2015}: Подумать о правильном поядке вызова при создании/обновлении/удалении факта (до/после - аналогично *DependentAggregateFeature или должен отличаться?)
    public class DependentStatisticsFeature<TFact> : IIndirectFactDependencyFeature, IFactDependencyFeature<TFact>
        where TFact : IIdentifiable<long>
    {
        public DependentStatisticsFeature(MapToObjectsSpecProvider<TFact, IOperation> mapSpecificationProvider)
        {
            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                  = MapSpecificationProviderOnDelete
                    = mapSpecificationProvider;
        }

        public Type DependencyType => typeof(TFact);

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnCreate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnUpdate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnDelete { get; }
    }
}