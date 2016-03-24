using System;
using System.Linq.Expressions;

namespace NuClear.River.Common.Metadata.Model
{
    /// <summary>
    /// Provides match between entity and its identifier
    /// </summary>
    /// <typeparam name="TKey">Identifier type</typeparam>
    public interface IIdentityProvider<TKey>
    {
        Expression<Func<TIdentifiable, TKey>> Get<TIdentifiable>()
            where TIdentifiable : IIdentifiable<TKey>;
    }
}