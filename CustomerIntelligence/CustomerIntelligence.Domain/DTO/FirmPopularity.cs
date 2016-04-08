using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class FirmPopularity : IBitDto
    {
        public long ProjectId { get; set; }

        public IReadOnlyCollection<Firm> Firms { get; set; }

        public class Firm
        {
            public long FirmId { get; set; }

            public IReadOnlyCollection<Category> Categories { get; set; }

            public class Category
            {
                public long CategoryId { get; set; }
                public int Hits { get; set; }
                public int Shows { get; set; }
            }
        }
    }
}