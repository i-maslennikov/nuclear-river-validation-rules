using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPosition, Storage.Model.Erm.AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Storage.Model.Erm.AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Storage.Model.Erm.Category>()
                .AddMapping<EntityTypeDeniedPosition, Storage.Model.Erm.DeniedPosition>()
                .AddMapping<EntityTypeRuleset, Storage.Model.Erm.Ruleset>()
                .AddMapping<EntityTypeOrder, Storage.Model.Erm.Order>()
                .AddMapping<EntityTypeOrderPosition, Storage.Model.Erm.OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Storage.Model.Erm.OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Storage.Model.Erm.OrganizationUnit>()
                .AddMapping<EntityTypePosition, Storage.Model.Erm.Position>()
                .AddMapping<EntityTypePrice, Storage.Model.Erm.Price>()
                .AddMapping<EntityTypePricePosition, Storage.Model.Erm.PricePosition>()
                .AddMapping<EntityTypeProject, Storage.Model.Erm.Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPosition, Storage.Model.Facts.AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Storage.Model.Facts.AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Storage.Model.Facts.Category>()
                .AddMapping<EntityTypeDeniedPosition, Storage.Model.Facts.DeniedPosition>()
                .AddMapping<EntityTypeRuleset, Storage.Model.Facts.RulesetRule>()
                .AddMapping<EntityTypeOrder, Storage.Model.Facts.Order>()
                .AddMapping<EntityTypeOrderPosition, Storage.Model.Facts.OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Storage.Model.Facts.OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Storage.Model.Facts.OrganizationUnit>()
                .AddMapping<EntityTypePosition, Storage.Model.Facts.Position>()
                .AddMapping<EntityTypePrice, Storage.Model.Facts.Price>()
                .AddMapping<EntityTypePricePosition, Storage.Model.Facts.PricePosition>()
                .AddMapping<EntityTypeProject, Storage.Model.Facts.Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AggregateTypeMap
            = builder => builder
                .AddMapping<EntityTypeOrder, Storage.Model.Aggregates.Order>()
                .AddMapping<EntityTypePrice, Storage.Model.Aggregates.Price>()
                .AddMapping<EntityTypeRuleset, Storage.Model.Aggregates.Ruleset>()
                .AddMapping<EntityTypePosition, Storage.Model.Aggregates.Position>()
                .AddMapping<EntityTypePeriod, Storage.Model.Aggregates.Period>()
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.AdvertisementAmountRestriction))
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.PriceAssociatedPosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.OrderPeriod))
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.OrderPosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.OrderPricePosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.Aggregates.PricePeriod));

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