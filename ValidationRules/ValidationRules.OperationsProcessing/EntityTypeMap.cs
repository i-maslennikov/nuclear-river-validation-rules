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
                .AddMapping<EntityTypeAssociatedPosition, Storage.Model.PriceRules.Facts.AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Storage.Model.PriceRules.Facts.AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Storage.Model.PriceRules.Facts.Category>()
                .AddMapping<EntityTypeDeniedPosition, Storage.Model.PriceRules.Facts.DeniedPosition>()
                .AddMapping<EntityTypeRuleset, Storage.Model.PriceRules.Facts.RulesetRule>()
                .AddMapping<EntityTypeOrder, Storage.Model.PriceRules.Facts.Order>()
                .AddMapping<EntityTypeOrderPosition, Storage.Model.PriceRules.Facts.OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Storage.Model.PriceRules.Facts.OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Storage.Model.PriceRules.Facts.OrganizationUnit>()
                .AddMapping<EntityTypePosition, Storage.Model.PriceRules.Facts.Position>()
                .AddMapping<EntityTypePrice, Storage.Model.PriceRules.Facts.Price>()
                .AddMapping<EntityTypePricePosition, Storage.Model.PriceRules.Facts.PricePosition>()
                .AddMapping<EntityTypeProject, Storage.Model.PriceRules.Facts.Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AggregateTypeMap
            = builder => builder
                .AddMapping<EntityTypeOrder, Storage.Model.PriceRules.Aggregates.Order>()
                .AddMapping<EntityTypePrice, Storage.Model.PriceRules.Aggregates.Price>()
                .AddMapping<EntityTypePosition, Storage.Model.PriceRules.Aggregates.Position>()
                .AddMapping<EntityTypePeriod, Storage.Model.PriceRules.Aggregates.Period>()
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.AdvertisementAmountRestriction))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPeriod))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPricePosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.PricePeriod));

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