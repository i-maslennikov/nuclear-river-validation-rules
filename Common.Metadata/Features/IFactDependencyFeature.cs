using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.River.Common.Metadata.Features
{
    public interface IFactDependencyFeature : IMetadataFeature
    {
        Type FactType { get; }
        Type EntityType { get; }
        DependencyType DependencyType { get; }
    }
}
