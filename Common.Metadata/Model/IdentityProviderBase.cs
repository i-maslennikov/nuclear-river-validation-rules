namespace NuClear.River.Common.Metadata.Model
{
    /// <summary>
    /// Предосталяет упрощённый доступ к экземпляру IIdentityProvider
    /// Необязательный базовый класс.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IdentityProviderBase<T>
        where T : new()
    {
        public static T Instance { get; } = new T();
    }
}