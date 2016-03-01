using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Domain.EntityTypes;

using Erm = NuClear.ValidationRules.Domain.Model.Erm;
using Facts = NuClear.ValidationRules.Domain.Model.Facts;
using Aggregates = NuClear.ValidationRules.Domain.Model.Aggregates;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPosition, Erm::AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Erm::AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Erm::Category>()
                .AddMapping<EntityTypeDeniedPosition, Erm::DeniedPosition>()
                .AddMapping<EntityTypeGlobalAssociatedPosition, Erm::GlobalAssociatedPosition>()
                .AddMapping<EntityTypeGlobalDeniedPosition, Erm::GlobalDeniedPosition>()
                .AddMapping<EntityTypeOrder, Erm::Order>()
                .AddMapping<EntityTypeOrderPosition, Erm::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Erm::OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Erm::OrganizationUnit>()
                .AddMapping<EntityTypePosition, Erm::Position>()
                .AddMapping<EntityTypePrice, Erm::Price>()
                .AddMapping<EntityTypePricePosition, Erm::PricePosition>()
                .AddMapping<EntityTypeProject, Erm::Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPosition, Facts::AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Facts::AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Facts::Category>()
                .AddMapping<EntityTypeDeniedPosition, Facts::DeniedPosition>()
                .AddMapping<EntityTypeOrder, Facts::Order>()
                .AddMapping<EntityTypeOrderPosition, Facts::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Facts::OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Facts::OrganizationUnit>()
                .AddMapping<EntityTypePosition, Facts::Position>()
                .AddMapping<EntityTypePrice, Facts::Price>()
                .AddMapping<EntityTypePricePosition, Facts::PricePosition>()
                .AddMapping<EntityTypeProject, Facts::Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AggregateTypeMap
            = builder => builder
                .AddMapping<EntityTypeOrder, Aggregates::Order>()
                .AddMapping<EntityTypePrice, Aggregates::Price>()
                .AddMapping<EntityTypePosition, Aggregates::Position>()
                .AddMapping<EntityTypePeriod, Aggregates::Period>()
                .AddAsPersistenceOnly(typeof(Aggregates::AdvertisementAmountRestriction))
                .AddAsPersistenceOnly(typeof(Aggregates::PriceDeniedPosition))
                .AddAsPersistenceOnly(typeof(Aggregates::PriceAssociatedPosition))
                .AddAsPersistenceOnly(typeof(Aggregates::OrderPeriod))
                .AddAsPersistenceOnly(typeof(Aggregates::OrderPosition))
                .AddAsPersistenceOnly(typeof(Aggregates::OrderPrice))
                .AddAsPersistenceOnly(typeof(Aggregates::PricePeriod));

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