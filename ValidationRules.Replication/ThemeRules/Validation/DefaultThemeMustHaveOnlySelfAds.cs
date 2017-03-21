using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ThemeRules.Validation
{
    /// <summary>
    /// Для заказов, с отличным от "самореклама" типом и продажей в тематику по-умолчанию, должна выводиться ошибка
    /// 
    /// "Установленная по умолчанию тематика {0} должна содержать только саморекламу"
    /// 
    /// Source: DefaultThemeMustContainOnlySelfAdvValidationRule/DeafaultThemeMustContainOnlySelfAds
    /// </summary>
    public sealed class DefaultThemeMustHaveOnlySelfAds : ValidationResultAccessorBase
    {
        public DefaultThemeMustHaveOnlySelfAds(IQuery query) : base(query, MessageTypeCode.DefaultThemeMustHaveOnlySelfAds)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from orderTheme in query.For<Order.OrderTheme>().Where(x => x.OrderId == order.Id)
                from theme in query.For<Theme>().Where(x => x.Id == orderTheme.ThemeId)
                where !order.IsSelfAds // не самореклама
                where theme.IsDefault // тематика по умолчанию
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeTheme>(theme.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDateFact,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
