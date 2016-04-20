using System;
using System.Collections.Generic;

using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.API.Metadata
{
    public sealed class BulkReplicationMetadataBuilder : MetadataElementBuilder<BulkReplicationMetadataBuilder, BulkReplicationMetadataElement>
    {
        private readonly List<Type> _dataObjectTypes = new List<Type>();
        private readonly List<string> _essentialViewNames = new List<string>();
        private string _commandLineKey;

        public BulkReplicationMetadataBuilder CommandlineKey(string key)
        {
            _commandLineKey = key;
            return this;
        }

        public BulkReplicationMetadataBuilder From(IConnectionStringIdentity connectionString, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.From,  connectionString, mappingSchema));
            return this;
        }

        public BulkReplicationMetadataBuilder To(IConnectionStringIdentity connectionString, MappingSchema mappingSchema)
        {
            AddFeatures(new StorageDescriptorFeature(ReplicationDirection.To, connectionString, mappingSchema));
            return this;
        }

        public BulkReplicationMetadataBuilder ForDataObject<TDataObject>()
        {
            _dataObjectTypes.Add(typeof(TDataObject));
            return this;
        }

        public BulkReplicationMetadataBuilder EssentialView(string viewName)
        {
            _essentialViewNames.Add(viewName);
            return this;
        }

        protected override BulkReplicationMetadataElement Create()
        {
            return new BulkReplicationMetadataElement(_commandLineKey, _dataObjectTypes,  _essentialViewNames, Features);
        }
    }
}