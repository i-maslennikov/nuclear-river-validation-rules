using LinqToDB.Mapping;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.StateInitialization.Core.Storage
{
    public sealed class StorageDescriptor
    {
        public StorageDescriptor(IConnectionStringIdentity connectionStringIdentity, MappingSchema mappingSchema)
        {
            ConnectionStringIdentity = connectionStringIdentity;
            MappingSchema = mappingSchema;
        }

        public IConnectionStringIdentity ConnectionStringIdentity { get; }
        public MappingSchema MappingSchema { get; }
    }
}