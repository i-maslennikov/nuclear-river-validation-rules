using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязан неактульный адрес, должна выводиться ошибка.
    /// "В позиции {0} адрес фирмы {1} скрыт навсегда"
    /// "В позиции {0} адрес фирмы {1} скрыт до выяснения"
    /// "В позиции {0} найден неактивный адрес {1}"
    /// "В позиции {0} найден адрес {1}, не принадлежащий фирме заказа"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// 
    /// Тут я позволил себе не выводить все сообщения, а только одно в следующем порядке: не принадлежит, навсегда, неактивен, до выяснения.
    /// </summary>
    public sealed class LinkedFirmAddressShouldBeValid : ValidationResultAccessorBase
    {
        public LinkedFirmAddressShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedFirmAddressShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from firmAddress in query.For<Order.InvalidFirmAddress>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(new Dictionary<string, object>
                                                  {
                                                      { "invalidFirmAddressState", (int)firmAddress.State },
                                                      { "isPartnerAddress", firmAddress.IsPartnerAddress }
                                                  },
                                              new Reference<EntityTypeFirmAddress>(firmAddress.FirmAddressId),
                                              new Reference<EntityTypeOrder>(order.Id),
                                              new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                                                                                  new Reference<EntityTypeOrderPosition>(firmAddress.OrderPositionId),
                                                                                                  new Reference<EntityTypePosition>(firmAddress.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}