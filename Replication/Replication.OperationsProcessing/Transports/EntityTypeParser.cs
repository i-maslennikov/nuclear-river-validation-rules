using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public sealed class EntityTypeParser<TSubDomain> : IEntityTypeParser
        where TSubDomain : ISubDomain
    {
        private readonly IEntityTypeMappingRegistry<TSubDomain> _typeMappingRegistry;

        public EntityTypeParser(IEntityTypeMappingRegistry<TSubDomain> typeMappingRegistry)
        {
            _typeMappingRegistry = typeMappingRegistry;
        }

        public IEntityType Parse(int id)
        {
            IEntityType result;
            return _typeMappingRegistry.TryParse(id, out result)
                       ? result
                       : new UnknownEntityType(id);
        }
    }
}