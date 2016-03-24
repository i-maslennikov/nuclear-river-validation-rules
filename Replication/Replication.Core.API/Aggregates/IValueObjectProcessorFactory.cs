using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessorFactory
    {
        IValueObjectProcessor Create(IValueObjectMetadata metadata);
    }
}