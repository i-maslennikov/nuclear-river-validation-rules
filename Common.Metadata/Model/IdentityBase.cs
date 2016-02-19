namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    /// <summary>
    /// Предосталяет упрощённый доступ к экземпляру IIdentity
    /// Необязательный базовый класс.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IdentityBase<T>
        where T : new()
    {
        public static T Instance { get; } = new T();
    }
}