using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Replication.DTO
{
    public sealed class RubricPopularity
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