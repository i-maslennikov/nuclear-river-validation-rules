using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class ChildEntityFeature<TRootKey, TChildEntity> : IMetadataFeature
    {
        public ChildEntityFeature(Func<IReadOnlyCollection<TRootKey>, FindSpecification<TChildEntity>> findSpecificationProvider)
        {
            FindSpecificationProvider = findSpecificationProvider;
        }

        public Func<IReadOnlyCollection<TRootKey>, FindSpecification<TChildEntity>> FindSpecificationProvider { get; }
    }
}