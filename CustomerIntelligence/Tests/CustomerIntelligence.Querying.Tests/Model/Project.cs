using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Querying.Tests.Model
{
    public sealed class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<Category> Categories { get; set; }
        public ICollection<Territory> Territories { get; set; }
        public ICollection<Firm> Firms { get; set; }
    }
}