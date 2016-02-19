using System;
using System.Linq.Expressions;

namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    /// <summary>
    /// Предосталяет метод для установления связи между сущностью и её идентификатором.
    /// </summary>
    /// <typeparam name="TKey">Тип идентификатора</typeparam>
    public interface IIdentity<TKey>
    {
        Expression<Func<TIdentifiable, TKey>> ExtractIdentity<TIdentifiable>();
    }
}