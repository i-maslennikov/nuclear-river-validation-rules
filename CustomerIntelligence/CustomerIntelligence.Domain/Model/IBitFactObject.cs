using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model
{
    public interface IBitFactObject : IFactObject
    {
        long ProjectId { get; set; }
        long CategoryId { get; set; }
    }
}