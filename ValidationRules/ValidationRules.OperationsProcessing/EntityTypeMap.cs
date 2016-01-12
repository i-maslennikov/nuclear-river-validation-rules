using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

using Erm = NuClear.ValidationRules.Domain.Model.Erm;
using Facts = NuClear.ValidationRules.Domain.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPositionsGroup, Erm::AssociatedPositionsGroup>()
                .AddMapping<EntityTypeAssociatedPosition, Erm::AssociatedPosition>()
                .AddMapping<EntityTypeDeniedPosition, Erm::DeniedPosition>()
                .AddMapping<EntityTypeOrder, Erm::Order>()
                .AddMapping<EntityTypeOrderPosition, Erm::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Erm::OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Erm::OrganizationUnit>()
                .AddMapping<EntityTypePrice, Erm::Price>()
                .AddMapping<EntityTypePricePosition, Erm::PricePosition>()
                .AddMapping<EntityTypeProject, Erm::Project>()
                .AddMapping<EntityTypePosition, Erm::Position>()
                .AddMapping<EntityTypeCategory, Erm::Category>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeOrder, Facts::Order>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AggregateTypeMap
            = builder => { };

        public static IEntityTypeMappingRegistry<ErmSubDomain> CreateErmContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ErmTypeMap.Invoke(builder);
            return builder.Create<ErmSubDomain>();
        }

        public static IEntityTypeMappingRegistry<FactsSubDomain> CreateFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            FactsTypeMap.Invoke(builder);
            return builder.Create<FactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AggregateSubDomain> CreateAggregateContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            AggregateTypeMap.Invoke(builder);
            return builder.Create<AggregateSubDomain>();
        }
    }

    public static class EntityTypeMappingRegistryBuilderExtensions
    {
        public static EntityTypeMappingRegistryBuilder AddMapping<TEntityType, TAggregateType>(this EntityTypeMappingRegistryBuilder registryBuilder)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            return registryBuilder.AddMapping(IdentityBase<TEntityType>.Instance, typeof(TAggregateType));
        }
    }
}