using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, у которых в объектах привязки встречаются не привязные к городу назначения рубрики, должна выводиться ошибка.
    /// "В позиции {0} задействованы рубрики, не привязанные к отделению организации города назначения заказа: {рубрики}"
    /// -> "Рубрика {0} используется в позиции {1}, но не привязана к отделению организации города назначения заказа"
    /// 
    /// Source: CategoriesLinkedToDestOrgUnitOrderValidationRule
    /// 
    /// * Рубрика первого уровня считается привязанной к городу, если првязана хотя бы одна из её дочерних.
    /// * Сделано изменение относительно ERM - в одном сообщении только одна рубрика
    /// </summary>
    public sealed class OrderMustUseCategoriesOnlyAvailableInProject : ValidationResultAccessorBase
    {
        public OrderMustUseCategoriesOnlyAvailableInProject(IQuery query) : base(query, MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from opa in query.For<Order.CategoryAdvertisement>().Where(x => x.OrderId == order.Id)
                let categoryBindedToProject = query.For<Project.Category>().Any(x => x.ProjectId == order.ProjectId && x.CategoryId == opa.CategoryId)
                where !categoryBindedToProject
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(opa.OrderPositionId),
                                        new Reference<EntityTypePosition>(opa.PositionId)),
                                    new Reference<EntityTypeCategory>(opa.CategoryId))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}