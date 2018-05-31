using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Connections;

namespace ValidationRules.Hosting.Common.Settings.Connections
{
    public static class ConnectionStrings
    {
        private static readonly IReadOnlyDictionary<IConnectionStringIdentity, string> Identities2NamesMap =
            new Dictionary<IConnectionStringIdentity, string>
            {
                [ErmConnectionStringIdentity.Instance] = Names.Erm,
                [AmsConnectionStringIdentity.Instance] = Names.Ams,
                [RulesetConnectionStringIdentity.Instance] = Names.Rulesets,
                [FactsConnectionStringIdentity.Instance] = Names.Facts,
                [AggregatesConnectionStringIdentity.Instance] = Names.Aggregates,
                [MessagesConnectionStringIdentity.Instance] = Names.Messages,
                [ServiceBusConnectionStringIdentity.Instance] = Names.ServiceBus,
                [InfrastructureConnectionStringIdentity.Instance] = Names.Infrastructure,
                [LoggingConnectionStringIdentity.Instance] = Names.Logging
            };

        public static class Names
        {
            public const string Erm = "Erm";
            public const string Ams = "Ams";
            public const string Rulesets = "Rulesets";
            public const string Facts = "Facts";
            public const string Aggregates = "Aggregates";
            public const string Messages = "Messages";
            public const string ServiceBus = "ServiceBus";
            public const string Infrastructure = "Infrastructure";
            public const string Logging = "Logging";
        }

        public static IReadOnlyDictionary<IConnectionStringIdentity, string> For(params IConnectionStringIdentity[] identities)
        {
            var unresolvedConnectionStrings = new HashSet<IConnectionStringIdentity>();
            var resolvedConnectionStrings = new Dictionary<IConnectionStringIdentity, string> ();
            foreach (var identity in identities.Distinct())
            {
                if (!Identities2NamesMap.TryGetValue(identity, out var connectionStringName))
                {
                    unresolvedConnectionStrings.Add(identity);
                    continue;
                }

                resolvedConnectionStrings.Add(identity, GetConnectionString(connectionStringName));
            }

            if (unresolvedConnectionStrings.Any())
            {
                string message = "Unresolved names for connection strings identities: "
                                 + string.Join(";", unresolvedConnectionStrings.Select(i => i.GetType().Name));
                throw new InvalidOperationException(message);
            }

            return resolvedConnectionStrings;
        }

        private static string GetConnectionString(string connnectionStringKey) =>
            ConfigurationManager.ConnectionStrings[connnectionStringKey].ConnectionString;
    }
}
