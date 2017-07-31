using System;
using System.Collections.Generic;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmConnectionStringIdentity>()
            .HasSchema(Schema.Erm);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsConnectionStringIdentity>()
            .HasSchema(Schema.Facts);

        private static readonly SchemaMetadataElement Aggregates = SchemaMetadataElement.Config
            .For(ContextName.Aggregates)
            .HasConnectionString<AggregatesConnectionStringIdentity>()
            .HasSchema(Schema.Aggregates);

        private static readonly SchemaMetadataElement Messages = SchemaMetadataElement.Config
            .For(ContextName.Messages)
            .HasConnectionString<MessagesConnectionStringIdentity>()
            .HasSchema(Schema.Messages);

        public SchemaMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { Erm.Identity.Id, Erm },
                            { Facts.Identity.Id, Facts },
                            { Aggregates.Identity.Id, Aggregates },
                            { Messages.Identity.Id, Messages},
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
