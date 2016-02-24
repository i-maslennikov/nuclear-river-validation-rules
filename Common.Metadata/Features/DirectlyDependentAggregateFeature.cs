using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Features
{
    public class DirectlyDependentAggregateFeature<T> : IFactDependencyFeature<T> where T : class, IIdentifiable
    {
        public DirectlyDependentAggregateFeature(
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnCreate,
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnUpdate,
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProviderOnDelete)

        {
            MapSpecificationProviderOnCreate = mapSpecificationProviderOnCreate;
            MapSpecificationProviderOnUpdate = mapSpecificationProviderOnUpdate;
            MapSpecificationProviderOnDelete = mapSpecificationProviderOnDelete;
            FindSpecificationProvider = Specs.Find.ByIds<T>;
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