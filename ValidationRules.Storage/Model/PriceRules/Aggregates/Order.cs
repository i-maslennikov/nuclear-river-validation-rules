using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность заказа
    /// </summary>
    public sealed class Order
    {
        public long Id { get; set; }

        /// <summary>
        /// Связь заказа с номенклатурной позицией, импортируется из ERM.OrderPosition + ERM.OrderPositionAdv
        /// </summary>
        // TODO: Сущность с последними изменениями стала достаточно узкоспециализированной, можно окончательно заточить её под решение конкретных задач и упростить проверку
        public sealed class OrderPosition
        {
            public long OrderId { get; set; }
            public long ItemPositionId { get; set; }

            public long? CategoryId { get; set; }
            public long? ThemeId { get; set; }
        }

        /// <summary>
        /// Связь заказа с его позицией и позицией прайс-листа
        /// </summary>
        public sealed class OrderPricePosition
        {
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public long PriceId { get; set; }
            public bool IsActive { get; set; }
        }

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

        public sealed class ActualPrice
        {
            public long OrderId { get; set; }
            public long? PriceId { get; set; }
        }
    }
}