using System;
using System.Collections.Generic;
using System.Globalization;

namespace NuClear.AdvancedSearch.Common.Metadata.Context
{
    public static class PredicateProperty
    {
        public static readonly PredicateProperty<string> Type = new PredicateProperty<string>("type");
        public static readonly PredicateProperty<int> EntityType = new PredicateProperty<int>("entityType");
        public static readonly PredicateProperty<long> EntityId = new PredicateProperty<long>("entityId");
        public static readonly PredicateProperty<long> ProjectId = new PredicateProperty<long>("projectId");
        public static readonly PredicateProperty<long> CategoryId = new PredicateProperty<long>("categoryId");
    }

    public sealed class PredicateProperty<T>
    {
        public PredicateProperty(string name)
        {
            Name = name;
            Type = typeof(T);
        }

        public string Name { get; }
        public Type Type { get; }

        public T GetValue(Predicate p)
        {
            string value;
            if (!p.Properties.TryGetValue(Name, out value))
            {
                throw new ArgumentException($"Property {Name} is missing in predicate {p.Id}");
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public void SetValue(IDictionary<string, string> properties, T value)
        {
            properties[Name] = Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}