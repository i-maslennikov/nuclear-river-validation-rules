using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Replication.OperationsProcessing.Metadata
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