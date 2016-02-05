using NuClear.Model.Common.Entities;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf.Surrogates;

using ProtoBuf.Meta;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class TrackedUseCaseConfigurator<TSubDomain> : IRuntimeTypeModelConfigurator where TSubDomain : ISubDomain
    {
        public TrackedUseCaseConfigurator(IEntityTypeMappingRegistry<TSubDomain> entityTypeMappingRegistry)
        {
            // Переопределяем значение, определённое в FPE с целью использовать model-common v2.0.0
            // Поэтому вызов должен быть после ProtoBufTypeModelForTrackedUseCaseConfigurator
            SurrogateFactory<EntityTypeSurrogate<TSubDomain>>.Factory = () => new EntityTypeSurrogate<TSubDomain>(entityTypeMappingRegistry);
        }

        public RuntimeTypeModel Configure(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(EntityTypeSurrogate<TSubDomain>), false)
                     .Add(1, "Id").SetFactory(SurrogateFactory<EntityTypeSurrogate<TSubDomain>>.Factory.Method);
            typeModel.Add(typeof(IEntityType), false).SetSurrogate(typeof(EntityTypeSurrogate<TSubDomain>));

            return typeModel;
        }
    }
}
