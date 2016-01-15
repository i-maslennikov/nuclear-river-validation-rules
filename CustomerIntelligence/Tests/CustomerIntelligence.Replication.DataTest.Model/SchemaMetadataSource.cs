using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.CustomerIntelligence.Storage;
using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmTestConnectionStringIdentity>()
            .HasMasterConnectionString<ErmMasterConnectionStringIdentity>()
            .HasSchema(Schema.Erm)
            .HasEntitiesFromNamespace(typeof(Erm::Account).Namespace);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Facts::Account).Namespace);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For(ContextName.CustomerIntelligence)
            .HasConnectionString<CustomerIntelligenceTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(CI::CategoryGroup).Namespace);

        private static readonly SchemaMetadataElement Bit = SchemaMetadataElement.Config
            .For(ContextName.Bit)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            // временно объединяем Bit и Facts в одну базу данных, потом надо будет опять разделить
            //.HasConnectionString<BitTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Bit::FirmCategoryStatistics).Namespace);

        private static readonly SchemaMetadataElement Statistics = SchemaMetadataElement.Config
            .For(ContextName.Statistics)
            .HasConnectionString<StatisticsTestConnectionStringIdentity>()
            .HasSchema(Schema.CustomerIntelligence)
            .HasEntitiesFromNamespace(typeof(Statistics::FirmCategory3).Namespace);

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
