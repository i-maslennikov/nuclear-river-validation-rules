using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class RubricPopularity : IBitDto
    {
        public long ProjectId { get; set; }

        public IReadOnlyCollection<Category> Categories { get; set; }

        public class Category
        {
            public long CategoryId { get; set; }

            public long AdvertisersCount { get; set; }
        }
    }
}