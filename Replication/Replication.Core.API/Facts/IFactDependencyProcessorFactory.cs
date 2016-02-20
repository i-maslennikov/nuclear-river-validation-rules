using NuClear.River.Common.Metadata.Features;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactDependencyProcessorFactory
    {
        IFactDependencyProcessor Create(IFactDependencyFeature metadata);
    }
}