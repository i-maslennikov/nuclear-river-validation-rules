using System;
using System.Linq.Expressions;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.River.Common.Metadata.Model
{
    public class StatisticsKeyIdentityProvider : IIdentityProvider<StatisticsKey>
    {
        private static readonly PropertyAutomapper<StatisticsKey> Automapper = new PropertyAutomapper<StatisticsKey>();

        public Expression<Func<TIdentifiable, StatisticsKey>> Get<TIdentifiable>()
            where TIdentifiable : IIdentifiable<StatisticsKey>
        {
            return Automapper.ExtractIdentity<TIdentifiable>();
        }
    }
}