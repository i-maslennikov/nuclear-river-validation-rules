using LinqToDB.Mapping;

namespace NuClear.StateInitialization.Core.Storage
{
    public sealed class StorageDescriptor
    {
        public StorageDescriptor(string connectionStringName, MappingSchema mappingSchema)
        {
            ConnectionStringName = connectionStringName;
            MappingSchema = mappingSchema;
        }

        public string ConnectionStringName { get; }
        public MappingSchema MappingSchema { get; }
    }
}