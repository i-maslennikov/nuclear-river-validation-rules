using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IValueObjectMetadataElement : IMetadataElement
    {
         Type ValueObjectType { get; }
    }
}