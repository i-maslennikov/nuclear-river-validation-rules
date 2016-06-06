using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.CustomerIntelligence.Storage;
using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmTestConnectionStringIdentity>()
            .HasMasterConnectionString<ErmMasterConnectionStringIdentity>()
            .HasSchema(Schema.Erm);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For(ContextName.CustomerIntelligence)
            .HasConnectionString<CustomerIntelligenceTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence);

        private static readonly SchemaMetadataElement Bit = SchemaMetadataElement.Config
            .For(ContextName.Bit)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts);

        private static readonly SchemaMetadataElement Statistics = SchemaMetadataElement.Config
            .For(ContextName.Statistics)
            .HasConnectionString<CustomerIntelligenceTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence);

        public SchemaMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { Erm.Identity.Id, Erm },
                            { Facts.Identity.Id, Facts },
                            { CustomerIntelligence.Identity.Id, CustomerIntelligence },
                            { Bit.Identity.Id, Bit },
                            { Statistics.Identity.Id, Statistics },
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
