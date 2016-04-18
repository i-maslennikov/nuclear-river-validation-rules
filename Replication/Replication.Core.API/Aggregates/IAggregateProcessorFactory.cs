using NuClear.Metamodeling.Elements;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateProcessorFactory
    {
        IAggregateProcessor Create(IMetadataElement metadata);
    }
}