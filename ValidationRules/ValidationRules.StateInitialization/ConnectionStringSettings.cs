using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization
{
    public sealed class StateInitializationConnectionStringSettings : ConnectionStringSettingsAspect
    {
        public StateInitializationConnectionStringSettings(ConnectionStringSettingsCollection configuration)
            : base(CreateConnectionStringMappings(configuration))
        {
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(ConnectionStringSettingsCollection configuration)
        {
            var available = configuration.Cast<ConnectionStringSettings>().ToDictionary(x => x.Name, x => x.ConnectionString);
            var known = new Dictionary<string, IConnectionStringIdentity>
                        {
                            { "Erm", ErmConnectionStringIdentity.Instance },
                            { "Facts", FactsConnectionStringIdentity.Instance },
                            { "OrderValidationConfig", OrderValidationConfigIdentity.Instance },
                            { "Aggs", AggregatesConnectionStringIdentity.Instance },
                        };

            return available.Join(known, x => x.Key, x => x.Key, (x, y) => Tuple.Create(y.Value, x.Value)).ToDictionary(x => x.Item1, x => x.Item2);
        }
    }
}