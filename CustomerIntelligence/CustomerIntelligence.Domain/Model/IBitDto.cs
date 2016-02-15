using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model
{
    public interface IBitDto : IDataTransferObject
    {
         long ProjectId { get; }
    }
}