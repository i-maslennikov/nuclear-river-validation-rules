using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.River.Common.Metadata.Model
{
    public sealed class PropertyAutomapper<TKey>
        where TKey : new()
    {
        public Expression<Func<TIdentifiable, TKey>> ExtractIdentity<TIdentifiable>()
        {
            var keyProperties = typeof(TKey).GetRuntimeProperties();
            var entityProperties = typeof(TIdentifiable).GetRuntimeProperties();

            var propertyPairs = keyProperties.Join(entityProperties,
                                                   property => new { property.Name, property.PropertyType },
                                                   property => new { property.Name, property.PropertyType },
                                                   (keyProperty, entityProperty) => new { keyProperty, entityProperty });

            var param = Expression.Parameter(typeof(TIdentifiable));
            var bindings = propertyPairs.Select(x => Expression.Bind(x.keyProperty, Expression.Property(param, x.entityProperty)));

            var body = Expression.MemberInit(Expression.New(typeof(TKey)), bindings);
            return Expression.Lambda<Func<TIdentifiable, TKey>>(body, param);
        }
    }
}