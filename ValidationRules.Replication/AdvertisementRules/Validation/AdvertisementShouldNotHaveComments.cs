using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, в которых есть РМ со state code = 3, должна выводиться ошибка
    /// "Рекламный материал {0} одобрен с замечаниями"
    /// </summary>
    public sealed class AdvertisementShouldNotHaveComments : ValidationResultAccessorBase
    {
        public AdvertisementShouldNotHaveComments(IQuery query) : base(query, MessageTypeCode.AdvertisementShouldNotHaveComments)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from fail in query.For<Order.AdvertisementFailedReview>()
                                  .Where(x => x.ReviewState == (int)Order.AdvertisementReviewState.OkWithComment)
                                  .Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "reviewState", fail.ReviewState} },
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeAdvertisement>(fail.AdvertisementId))
                                .ToXDocument(),

                    PeriodStart = order.BeginDistributionDate,
                    PeriodEnd = order.EndDistributionDatePlan,
                    OrderId = order.Id,
                };

            return ruleResults;
        }
    }
}