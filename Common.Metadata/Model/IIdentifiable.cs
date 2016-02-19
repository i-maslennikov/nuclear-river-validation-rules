namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    /// <summary>
    /// Маркерный интерфейс, означающий, сущность может быть сопосталена с идентификатором.
    /// </summary>
    /// <typeparam name="TIdentity">Тип, содержащий реализацию соответствия ключа и сущности</typeparam>
    /// <typeparam name="TKey">Тип ключа, используемый для помеченной маркером сущности</typeparam>
    public interface IIdentifiable<TIdentity, TKey>
        where TIdentity : IIdentity<TKey>
    {
    }
}