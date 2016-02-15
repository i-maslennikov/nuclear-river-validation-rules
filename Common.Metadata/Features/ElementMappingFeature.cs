using System;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.River.Common.Metadata.Features
{
    public sealed class ElementMappingFeature : IUniqueMetadataFeature
    {
        private readonly IMetadataElement _mappedElement;

        public ElementMappingFeature(IMetadataElement mappedElement)
        {
            if (mappedElement == null)
            {
                throw new ArgumentNullException("mappedElement");
            }
            _mappedElement = mappedElement;
        }

        public IMetadataElement MappedElement
        {
            get
            {
                return _mappedElement;
            }
        }
    }
}