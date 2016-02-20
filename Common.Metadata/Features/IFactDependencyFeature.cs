using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public interface IFactDependencyFeature : IMetadataFeature
    {
        Type DependancyType { get; }
    }

    public interface IIndirectFactDependencyFeature : IFactDependencyFeature
    {
    }

    public interface IFactDependencyFeature<T, TKey> : IFactDependencyFeature
    {
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnCreate { get; }
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnUpdate { get; }
        MapToObjectsSpecProvider<T, IOperation> MapSpecificationProviderOnDelete { get; }

        Func<IReadOnlyCollection<TKey>, FindSpecification<T>> FindSpecificationProvider { get; }
    }
}