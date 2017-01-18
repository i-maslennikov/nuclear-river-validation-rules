using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.WebApp.Model
{
    public class MessageContainerModel : ModelBase
    {
        public IEnumerable<Message> Results { get; set; }

        public IDictionary<Key, Message[]> ByRule()
            => Results.GroupBy(x => new Key { Level = x.Level, Class = x.Class, Rule = x.Rule })
                      .OrderByDescending(x => x.Key.Level)
                      .ToDictionary(x => x.Key, x => x.ToArray());

        public struct Key
        {
            public int Rule { get; set; }
            public Level Level { get; set; }
            public string Class { get; set; }
        }
    }
}