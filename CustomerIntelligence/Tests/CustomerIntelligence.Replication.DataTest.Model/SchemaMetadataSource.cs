using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

using CategoryGroup = NuClear.CustomerIntelligence.Storage.Model.CI.CategoryGroup;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmTestConnectionStringIdentity>()
            .HasMasterConnectionString<ErmMasterConnectionStringIdentity>()
            .HasSchema(Schema.Erm)
            .HasEntitiesFromNamespace(typeof(Account).Namespace);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Storage.Model.Facts.Account).Namespace);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For(ContextName.CustomerIntelligence)
            .HasConnectionString<CustomerIntelligenceTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(CategoryGroup).Namespace);

        private static readonly SchemaMetadataElement Bit = SchemaMetadataElement.Config
            .For(ContextName.Bit)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(FirmCategoryStatistics).Namespace);

        private static readonly SchemaMetadataElement Statistics = SchemaMetadataElement.Config
            .For(ContextName.Statistics)
            .HasConnectionString<CustomerIntelligenceTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(FirmCategory3).Namespace);

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
