using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.WebApp.Model
{
    public class MessageContainerModel : ModelBase
    {
        public IEnumerable<Message> Results { get; set; }

        public IDictionary<Key, Message[]> ByRule()
            => Results.GroupBy(x => new Key { Level = x.Level, Rule = x.Rule })
                      .OrderBy(x => x.Key.Level)
                      .ToDictionary(x => x.Key, x => x.ToArray());

        public struct Key
        {
            public int Rule { get; set; }
            public string Level { get; set; }
        }
    }
}