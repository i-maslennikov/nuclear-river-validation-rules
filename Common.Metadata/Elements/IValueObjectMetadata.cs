using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IValueObjectMetadata : IMetadataElement
    {
         Type ValueObjectType { get; }
         Type EntityKeyType { get; }
    }
}