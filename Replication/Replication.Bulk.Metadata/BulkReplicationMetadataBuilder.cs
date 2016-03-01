using System.Collections.Generic;

using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.References;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.Metadata
{
    public sealed class BulkReplicationMetadataBuilder : MetadataElementBuilder<BulkReplicationMetadataBuilder, BulkReplicationMetadataElement>
    {
        private readonly List<string> _essentialViewNames = new List<string>();
        private string _commandLineKey;

        public BulkReplicationMetadataBuilder CommandlineKey(string key)
        {
            _commandLineKey = key;
            return this;
        }

        public BulkReplicationMetadataBuilder From(IConnectionStringIdentity connectionString, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.From, connectionString, mappingSchema));
            return this;
        }

        public BulkReplicationMetadataBuilder To(IConnectionStringIdentity connectionString, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.To, connectionString, mappingSchema));
            return this;
        }

        public BulkReplicationMetadataBuilder UsingMetadataOfKind<TIdentity>(params string[] segments)
            where TIdentity : MetadataKindIdentityBase<TIdentity>, new()
        {
            var referencedElementId = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<TIdentity>().Segments(segments);
            Childs(MetadataReference.Config.For(referencedElementId));
            return this;
        }

        public BulkReplicationMetadataBuilder EssentialView(string viewName)
        {
            _essentialViewNames.Add(viewName);
            return this;
        }

        protected override BulkReplicationMetadataElement Create()
        {
            return new BulkReplicationMetadataElement(_commandLineKey, _essentialViewNames, Features);
        }
    }
}