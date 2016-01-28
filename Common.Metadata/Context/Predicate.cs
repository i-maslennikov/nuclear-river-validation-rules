using System.Collections.Generic;
using System.Linq;

namespace NuClear.AdvancedSearch.Common.Metadata.Context
{
    public class Predicate
    {
        public Predicate(IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Predicate> childs)
        {
            Properties = properties;
            Childs = childs;
        }

        public string Id => Properties["id"];
        public IReadOnlyDictionary<string, string> Properties { get; }
        public IReadOnlyCollection<Predicate> Childs { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var predicate = obj as Predicate;
            if (predicate == null)
            {
                return false;
            }

            return Equals(predicate);
        }

        public override int GetHashCode()
        {
            return Childs.Aggregate(1, (accumulator, child) => (accumulator ^ child.GetHashCode()) * 397) ^
                   Properties.Aggregate(1, (accumulator, pair) => (accumulator ^ pair.Key.GetHashCode() ^ pair.Value.GetHashCode()) * 397);
        }

        private bool Equals(Predicate other)
        {
            return Childs.Count == other.Childs.Count
                   && Childs.Zip(other.Childs, Equals).All(x => x) // Для дочерних предикатов в общем случае порядок важен
                   && Properties.Count == other.Properties.Count
                   && Properties.All(pair =>
                                         {
                                             string otherValue;
                                             other.Properties.TryGetValue(pair.Key, out otherValue);
                                             return Equals(pair.Value, otherValue);
                                         }); // Для свойств порядок не важен
        }
    }
}