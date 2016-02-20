using System;
using System.Linq.Expressions;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Неявная сущность из ERM, элементарные периоды размещения заказа/наличия прайс-листа.
    /// Должна расчитываться динамически, например, для единственного заведённого в системе заказа любой продолжительности будет один период.
    /// И в то же время, если заказы начнут размещаться посуточно/почасово - у одного заказа может быть множество периодов.
    /// 
    /// Должен соблюдаться инвариант: сумма всех периодов заказа/прайс-диста - неразрывна.
    /// </summary>
    public sealed class Period : IAggregateRoot, IIdentifiable<PeriodId>
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public sealed class PeriodId
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    /// <summary>
    /// Описывает идентификацию по составному ключу ComplexKey
    /// </summary>
    public class PeriodIdentityProvider : IdentityProviderBase<PeriodIdentityProvider>, IIdentityProvider<PeriodId>
    {
        private static readonly PropertyAutomapper<PeriodId> x;

        public Expression<Func<TIdentifiable, PeriodId>> ExtractIdentity<TIdentifiable>() 
            where TIdentifiable : IIdentifiable<PeriodId>
        {
            return x.ExtractIdentity<TIdentifiable>();
        }
    }
}
