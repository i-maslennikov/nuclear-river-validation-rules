using System;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model
{
    /// <summary>
    /// Описывает идентификацию по составному ключу ComplexKey
    /// </summary>
    public class PeriodIdentityProvider : IdentityProviderBase<PeriodIdentityProvider>, IIdentityProvider<PeriodId>
    {
        private static readonly PropertyAutomapper<PeriodId> Automapper = new PropertyAutomapper<PeriodId>();

        public Expression<Func<TIdentifiable, PeriodId>> ExtractIdentity<TIdentifiable>() 
            where TIdentifiable : IIdentifiable<PeriodId>
        {
            return Automapper.ExtractIdentity<TIdentifiable>();
        }
    }
}