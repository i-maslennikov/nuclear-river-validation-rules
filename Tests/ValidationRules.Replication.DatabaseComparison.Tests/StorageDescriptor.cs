using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage;

namespace ValidationRules.Replication.DatabaseComparison.Tests
{
    public sealed class StorageDescriptor
    {
        public static StorageDescriptor Erm = new StorageDescriptor(Schema.Erm);
        public static StorageDescriptor Facts = new StorageDescriptor(Schema.Facts);
        public static StorageDescriptor Aggregates = new StorageDescriptor(Schema.Aggregates);
        public static StorageDescriptor Messages = new StorageDescriptor(Schema.Messages);

        public StorageDescriptor(MappingSchema mappingSchema)
        {
            MappingSchema = mappingSchema;
            ConnectionStringName = mappingSchema.ConfigurationList[0];
        }

        public string ConnectionStringName { get; }
        public MappingSchema MappingSchema { get; }
    }
}