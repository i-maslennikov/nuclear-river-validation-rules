using LinqToDB.Mapping;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.API.Metadata
{
    public class StorageDescriptorFeature : IMetadataFeature
    {
        public StorageDescriptorFeature(ReplicationDirection direction, IConnectionStringIdentity connectionStringName, MappingSchema mappingSchema)
        {
            Direction = direction;
            ConnectionStringName = connectionStringName;
            MappingSchema = mappingSchema;
        }

        public ReplicationDirection Direction { get; private set; }

        public IConnectionStringIdentity ConnectionStringName { get; private set; }

        public MappingSchema MappingSchema { get; private set; }
    }
}