using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

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
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderMustUseCategoriesOnlyAvailableInProject(IQuery query) : base(query, MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from opa in query.For<Order.CategoryAdvertisement>().Where(x => x.OrderId == order.Id)
                from position in query.For<Position>().Where(x => x.Id == opa.PositionId)
                from category in query.For<Category>().Where(x => x.Id == opa.CategoryId)
                let categoryBindedToProject = query.For<Project.Category>().Any(x => x.ProjectId == order.ProjectId && x.CategoryId == opa.CategoryId)
                where !categoryBindedToProject
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)),
                                new XElement("orderPosition",
                                    new XAttribute("id", opa.OrderPositionId),
                                    new XAttribute("name", position.Name)),
                                new XElement("category",
                                    new XAttribute("id", category.Id),
                                    new XAttribute("name", category.Name)))),
                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        ProjectId = order.ProjectId,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}