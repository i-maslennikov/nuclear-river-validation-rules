using LinqToDB.Mapping;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.StateInitialization.Core.Storage
{
    public sealed class StorageDescriptor
    {
        public StorageDescriptor(IConnectionStringIdentity connectionString, MappingSchema mappingSchema)
        {
            ConnectionString = connectionString;
            MappingSchema = mappingSchema;
        }

        public IConnectionStringIdentity ConnectionString { get; }
        public MappingSchema MappingSchema { get; }
    }
}