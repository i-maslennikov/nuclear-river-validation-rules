using NuClear.Model.Common.Entities;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf.Surrogates;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    internal sealed class EntityTypeSurrogate<TSubDomain> where TSubDomain : ISubDomain
    {
        private readonly IEntityTypeMappingRegistry<TSubDomain> _entityTypeMappingRegistry;

        public EntityTypeSurrogate(IEntityTypeMappingRegistry<TSubDomain> entityTypeMappingRegistry)
        {
            _entityTypeMappingRegistry = entityTypeMappingRegistry;
        }

        public int Id { get; set; }

        [ProtoConverter]
        public static IEntityType From(EntityTypeSurrogate<TSubDomain> value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.Id == EntityTypeNone.Instance.Id)
            {
                return EntityTypeNone.Instance;
            }

            if (value.Id == EntityTypeAll.Instance.Id)
            {
                return EntityTypeAll.Instance;
            }

            IEntityType entityType;
            if (!value._entityTypeMappingRegistry.TryParse(value.Id, out entityType))
            {
                entityType = new UnknownEntityType(value.Id);
            }

            return entityType;
        }

        [ProtoConverter]
        public static EntityTypeSurrogate<TSubDomain> To(IEntityType value)
        {
            if (value == null)
            {
                return null;
            }

            var surrogate = SurrogateFactory<EntityTypeSurrogate<TSubDomain>>.Factory();
            surrogate.Id = value.Id;

            return surrogate;
        }
    }
}