using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.Storage.API.Specifications;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class Bit
            {
                public static partial class FirmCategoryStatistics
                {
                    public static FindSpecification<Bit::FirmCategoryStatistics> ByBitDto(IBitDto dto)
                    {
                        return new FindSpecification<Bit::FirmCategoryStatistics>(x => x.ProjectId == dto.ProjectId);
                    }
                }

                public static class ProjectCategoryStatistics
                {
                    public static FindSpecification<Bit::ProjectCategoryStatistics> ByBitDto(IBitDto dto)
                    {
                        return new FindSpecification<Bit::ProjectCategoryStatistics>(x => x.ProjectId == dto.ProjectId);
                    }
                }
            }
        }
    }
}