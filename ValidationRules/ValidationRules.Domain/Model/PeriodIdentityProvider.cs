using System;
using System.Linq.Expressions;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model
{
    public class PeriodIdentityProvider : IIdentityProvider<PeriodKey>
    {
        private static readonly PropertyAutomapper<PeriodKey> Automapper = new PropertyAutomapper<PeriodKey>();

        public Expression<Func<TIdentifiable, PeriodKey>> Get<TIdentifiable>()
            where TIdentifiable : IIdentifiable<PeriodKey>
        {
            return Automapper.ExtractIdentity<TIdentifiable>();
        }
    }
}