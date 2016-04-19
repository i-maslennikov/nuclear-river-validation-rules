using System;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Features
{
    public interface IFactDependencyFeature : IMetadataFeature
    {
        Type FactType { get; }
        IEntityType EntityType { get; }
    }
}
