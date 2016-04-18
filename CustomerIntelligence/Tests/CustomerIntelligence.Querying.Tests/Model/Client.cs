using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Querying.Tests.Model
{
    public sealed class Client
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CategoryGroup CategoryGroup { get; set; }
        public ICollection<ClientContact> Contacts { get; set; }
    }
}