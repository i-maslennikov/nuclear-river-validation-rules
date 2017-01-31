using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность заказа
    /// </summary>
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }

        /// <summary>
        /// Связь заказа с номенклатурной позицией, импортируется из ERM.OrderPosition + ERM.OrderPositionAdv
        /// </summary>
        public sealed class OrderPosition
        {
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PackagePositionId { get; set; }
            public long ItemPositionId { get; set; }

            public bool HasNoBinding { get; set; }
            public long? Category3Id { get; set; }
            public long? Category1Id { get; set; }
            public long? FirmAddressId { get; set; }
            public long? ThemeId { get; set; }
        }

        public sealed class OrderAssociatedPosition : IBindingObject
        {
            public long OrderId { get; set; }
            public long CauseOrderPositionId { get; set; }
            public long CausePackagePositionId { get; set; }
            public long CauseItemPositionId { get; set; }

            public long PrincipalPositionId { get; set; }
            public long BindingType { get; set; }

            public bool HasNoBinding { get; set; }
            public long? Category3Id { get; set; }
            public long? FirmAddressId { get; set; }
            public long? Category1Id { get; set; }

            public PositionSources Source { get; set; }
        }

        /// <summary>
        /// Представляет запрет, порождаемый одной из позиций заказа.
        /// Запрет распространяется не только на позиции заказа, к которому он привязан,
        /// но и ко всем заказа того же периода той же фирмы.
        /// </summary>
        public sealed class OrderDeniedPosition : IBindingObject
        {
            public long OrderId { get; set; }
            public long CauseOrderPositionId { get; set; }
            public long CausePackagePositionId { get; set; }
            public long CauseItemPositionId { get; set; }

            public long DeniedPositionId { get; set; }
            public long BindingType { get; set; }

            public bool HasNoBinding { get; set; }
            public long? Category3Id { get; set; }
            public long? FirmAddressId { get; set; }
            public long? Category1Id { get; set; }

            public PositionSources Source { get; set; }
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
    }

    [Flags]
    public enum PositionSources
    {
        None = 0,

        Opa = 1,
        Pkg = 1 << 1,

        Price = 1 << 2,
        Ruleset = 1 << 3,
    }
}