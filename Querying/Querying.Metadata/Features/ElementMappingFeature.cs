using System;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Querying.Metadata.Features
{
    public sealed class ElementMappingFeature : IUniqueMetadataFeature
    {
        public ElementMappingFeature(IMetadataElement mappedElement)
        {
            if (mappedElement == null)
            {
                throw new ArgumentNullException(nameof(mappedElement));
            }

            MappedElement = mappedElement;
        }

        public IMetadataElement MappedElement { get; }
    }
}