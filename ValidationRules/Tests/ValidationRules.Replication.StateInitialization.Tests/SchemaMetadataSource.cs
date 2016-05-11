using System;
using System.Collections.Generic;

using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.ValidationRules.Domain.Specifications;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public class SchemaMetadataSource : MetadataSourceBase<SchemaMetadataIdentity>
    {
        private static readonly SchemaMetadataElement Erm = SchemaMetadataElement.Config
            .For(ContextName.Erm)
            .HasConnectionString<ErmTestConnectionStringIdentity>()
            .HasSchema(Schema.Erm)
            .HasEntitiesFromNamespace(typeof(Order).Namespace);

        private static readonly SchemaMetadataElement Facts = SchemaMetadataElement.Config
            .For(ContextName.Facts)
            .HasConnectionString<FactsTestConnectionStringIdentity>()
            .HasSchema(Schema.Facts)
            .HasEntitiesFromNamespace(typeof(Storage.Model.Facts.Order).Namespace);

        private static readonly SchemaMetadataElement CustomerIntelligence = SchemaMetadataElement.Config
            .For(ContextName.Aggregates)
            .HasConnectionString<AggregatesTestConnectionStringIdentity>()
            .HasSchema(Schema.Aggregates)
            .HasEntitiesFromNamespace(typeof(Storage.Model.Aggregates.Order).Namespace);

        public SchemaMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { Erm.Identity.Id, Erm },
                            { Facts.Identity.Id, Facts },
                            { CustomerIntelligence.Identity.Id, CustomerIntelligence },
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
