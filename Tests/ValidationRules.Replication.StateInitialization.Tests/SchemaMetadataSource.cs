using System;
using System.Collections.Generic;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmTestConnectionStringIdentity>()
            .HasSchema(Schema.Erm);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For(ContextName.Aggregates)
            .HasConnectionString<AggregatesTestConnectionStringIdentity>()
            .HasSchema(Schema.Aggregates);

        private static readonly SchemaMetadataElement Messages = SchemaMetadataElement.Config
            .For(ContextName.Messages)
            .HasConnectionString<MessagesTestConnectionStringIdentity>()
            .HasSchema(Schema.Messages);

        public SchemaMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { Erm.Identity.Id, Erm },
                            { Facts.Identity.Id, Facts },
                            { CustomerIntelligence.Identity.Id, CustomerIntelligence },
                            { Messages.Identity.Id, Messages},
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
