using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IChildEntityFeature : IMetadataFeature
    {
        Type ChildEntityType { get; }
        Type ChildEntityKeyType { get; }
    }

    public class ChildEntityFeature<TRootKey, TChildEntity, TChildEntityKey> : IChildEntityFeature
    {
        public ChildEntityFeature(Func<IReadOnlyCollection<TRootKey>, FindSpecification<TChildEntity>> findSpecificationProvider)
        {
            FindSpecificationProvider = findSpecificationProvider;
        }

        public Type ChildEntityType
            => typeof(TChildEntity);

        public Type ChildEntityKeyType
            => typeof(TChildEntityKey);

        public Func<IReadOnlyCollection<TRootKey>, FindSpecification<TChildEntity>> FindSpecificationProvider { get; }
    }
}