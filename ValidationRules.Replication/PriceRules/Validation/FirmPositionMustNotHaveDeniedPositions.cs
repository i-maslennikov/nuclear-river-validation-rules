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
    /// Для заказов, которые содержат запрещённые друг к другу позиции должна выводиться ошибка
    /// (ошибка не должна выводиться, для одобренного заказа, если запрещённая позиция находится в не одобренном заказе)
    /// "{0} является запрещённой для: {1}"
    /// "{0} окажется запрещённой для: {1}"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/ADPCheckModeSpecificOrder_MessageTemplate
    ///         AssociatedAndDeniedPricePositionsOrderValidationRule/ADPCheckModeOrderBeingReapproved_MessageTemplate
    /// Когда заказ переведён "на расторжение", он не должен мешать создать другой заказ с конфликтующей позицией, но возврат в размещение должно быть невозможно.
    /// </summary>
    public sealed class FirmPositionMustNotHaveDeniedPositions : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmPositionMustNotHaveDeniedPositions(IQuery query) : base(query, MessageTypeCode.FirmPositionMustNotHaveDeniedPositions)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var errors =
                query.For<Firm.FirmPosition>()
                     .SelectMany(Specs.Join.Aggs.DeniedPositions(query.For<Firm.FirmDeniedPosition>(), query.For<Firm.FirmPosition>()), (position, denied) => new { position, denied })
                     .Where(dto => dto.denied.IsBindingObjectConditionSatisfied)
                     .Where(dto => dto.position.OrderPositionId != dto.denied.Position.OrderPositionId)
                     .Select(dto => new { position = dto.position, denied = dto.denied.Position });

            var messages =
                from conflict in errors
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(conflict.position.OrderPositionId,
                                        new Reference<EntityTypeOrder>(conflict.position.OrderId),
                                        new Reference<EntityTypePosition>(conflict.position.PackagePositionId),
                                        new Reference<EntityTypePosition>(conflict.position.ItemPositionId)),

                                    new Reference<EntityTypeOrderPosition>(conflict.denied.OrderPositionId,
                                        new Reference<EntityTypeOrder>(conflict.denied.OrderId),
                                        new Reference<EntityTypePosition>(conflict.denied.PackagePositionId),
                                        new Reference<EntityTypePosition>(conflict.denied.ItemPositionId)))
                                .ToXDocument(),

                    PeriodStart = conflict.position.Begin,
                    PeriodEnd = conflict.position.End,
                    OrderId = conflict.position.OrderId,

                    Result = RuleResult,
                };

            return messages;
        }
    }
}
