using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказа, в котором есть сопутствующая позиция и нет основной должна выводиться ошибка.
    /// "{0} является сопутствующей, основная позиция не найдена"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/AssociatedPositionWithoutPrincipalTemplate
    /// 
    /// Q1: Может ли элемент пакета быть удовлетворён другим элементом того же самого пакета?
    /// 
    /// Q2: Может ли пакет быть удовлетворён своим элементом или наоборот?
    /// </summary>
    // todo: переименовать PrincipalPositionMustExistForAssociated
    public sealed class AssociatedPositionWithoutPrincipal : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AssociatedPositionWithoutPrincipal(IQuery query) : base(query, MessageTypeCode.AssociatedPositionWithoutPrincipal)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var errors =
                query.For<Firm.FirmPosition>()
                     .Select(Specs.Join.Aggs.WithPrincipalPositions(query.For<Firm.FirmAssociatedPosition>(), query.For<Firm.FirmPosition>()))
                     .Where(dto => dto.RequirePrincipal && !dto.Principals.Any())
                     .Select(dto => dto.Associated);

            var messages =
                from error in errors
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(error.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.OrderId),
                                        new Reference<EntityTypePosition>(error.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.ItemPositionId)))
                                .ToXDocument(),

                    PeriodStart = error.Begin,
                    PeriodEnd = error.End,
                    OrderId = error.OrderId,

                    Result = RuleResult,
                };

            return messages;
        }
    }
}
