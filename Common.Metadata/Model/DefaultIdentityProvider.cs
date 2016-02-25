using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.River.Common.Metadata.Model
{
    /// <summary>
    /// Описывает идентификацию по единственному свойству Id типа int
    /// </summary>
    public class DefaultIdentityProvider : IdentityProviderBase<DefaultIdentityProvider>, IIdentityProvider<long>
    {
        private const string IdPropertyName = "Id";

        public Expression<Func<TIdentifiable, long>> ExtractIdentity<TIdentifiable>()
            where TIdentifiable : IIdentifiable<long>
        {
            var property = typeof(TIdentifiable).GetRuntimeProperty(IdPropertyName);
            var param = Expression.Parameter(typeof(TIdentifiable), "p");
            return Expression.Lambda<Func<TIdentifiable, long>>(Expression.Property(param, property), param);
        }
    }
}