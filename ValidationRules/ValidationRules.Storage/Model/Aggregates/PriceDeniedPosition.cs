namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// Описание пары запрещённых друг к другу позиций.
    /// </summary>
    public sealed class PriceDeniedPosition
    {
        // todo: Если в заказах есть OrderDeniedPosition, то эту таблицу можно удалить?
        public long PriceId { get; set; }

        public long DeniedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}