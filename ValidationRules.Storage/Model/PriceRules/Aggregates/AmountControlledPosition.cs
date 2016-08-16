namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Представляет продажу в заказе,
    /// относящуюся к категории номенклатурных позиций, количество которых долно контроллироваться.
    /// 
    /// Должен пересчитываться, если:
    ///     изменилось свойство IsControlledByAmount у номенклатурной позиции (добавляется/удаляется объект)
    ///     изменилось свойство CategoryCode у номенклатурной позиции
    ///     изменился OPA
    ///     изменился OP
    /// </summary>
    public sealed class AmountControlledPosition
    {
        public long OrderId { get; set; }
        public long CategoryCode { get; set; }
    }
}