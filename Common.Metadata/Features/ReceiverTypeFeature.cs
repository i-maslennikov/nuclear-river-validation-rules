using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.River.Common.Metadata.Features
{
    public class ReceiverTypeFeature : IUniqueMetadataFeature
    {
        public ReceiverTypeFeature(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}