using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность заказа
    /// </summary>
    public sealed class Order
    {
        public long Id { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistributionPlan { get; set; }
        public bool IsCommitted { get; set; }

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
        /// Продажа "Объявление в рубрике", чьё количество должно быть ограниено
        /// </summary>
        public sealed class OrderCategoryPosition
        {
            public long OrderId { get; set; }
            public long ProjectId { get; set; }
            public long OrderPositionAdvertisementId { get; set; }
            public long CategoryId { get; set; }
        }

        /// <summary>
        /// Продажа тематики
        /// </summary>
        public sealed class OrderThemePosition
        {
            public long OrderId { get; set; }
            public long ProjectId { get; set; }
            public long OrderPositionAdvertisementId { get; set; }
            public long ThemeId { get; set; }
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
            public long ProjectId { get; set; }
            public long CategoryCode { get; set; }
        }

        public sealed class ActualPrice
        {
            public long OrderId { get; set; }
            public long? PriceId { get; set; }
        }

        public sealed class OrderPeriod
        {
            public long OrderId { get; set; }
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public long Scope { get; set; }
        }

        public sealed class EntranceControlledPosition
        {
            public long OrderId { get; set; }
            public long EntranceCode { get; set; }
            public long FirmAddressId { get; set; }
        }
    }
}