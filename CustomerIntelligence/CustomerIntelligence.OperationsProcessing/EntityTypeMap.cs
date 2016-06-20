using System;

using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                    .AddMapping<EntityTypeAppointment, Storage.Model.Erm.Appointment>()
                    .AddMapping<EntityTypePhonecall, Storage.Model.Erm.Phonecall>()
                    .AddMapping<EntityTypeTask, Storage.Model.Erm.Task>()
                    .AddMapping<EntityTypeLetter, Storage.Model.Erm.Letter>()
                    .AddMapping<EntityTypeAccount, Storage.Model.Erm.Account>()
                    .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Storage.Model.Erm.BranchOfficeOrganizationUnit>()
                    .AddMapping<EntityTypeCategory, Storage.Model.Erm.Category>()
                    .AddMapping<EntityTypeCategoryFirmAddress, Storage.Model.Erm.CategoryFirmAddress>()
                    .AddMapping<EntityTypeCategoryGroup, Storage.Model.Erm.CategoryGroup>()
                    .AddMapping<EntityTypeCategoryOrganizationUnit, Storage.Model.Erm.CategoryOrganizationUnit>()
                    .AddMapping<EntityTypeClient, Storage.Model.Erm.Client>()
                    .AddMapping<EntityTypeContact, Storage.Model.Erm.Contact>()
                    .AddMapping<EntityTypeFirm, Storage.Model.Erm.Firm>()
                    .AddMapping<EntityTypeLead, Storage.Model.Erm.Lead>()
                    .AddMapping<EntityTypeFirmAddress, Storage.Model.Erm.FirmAddress>()
                    .AddMapping<EntityTypeFirmContact, Storage.Model.Erm.FirmContact>()
                    .AddMapping<EntityTypeLegalPerson, Storage.Model.Erm.LegalPerson>()
                    .AddMapping<EntityTypeOrder, Storage.Model.Erm.Order>()
                    .AddMapping<EntityTypeProject, Storage.Model.Erm.Project>()
                    .AddMapping<EntityTypeTerritory, Storage.Model.Erm.Territory>()
                    .AddMapping<EntityTypeSalesModelCategoryRestriction, Storage.Model.Erm.SalesModelCategoryRestriction>()
                    .AddAsVirtual(EntityTypeBuilding.Instance)
                    .AddAsVirtual(EntityTypeDeal.Instance)
                    .AddAsVirtual(EntityTypeOrderPosition.Instance)
                    .AddAsVirtual(EntityTypeBill.Instance)
                    .AddAsVirtual(EntityTypeLegalPersonProfile.Instance)
                    .AddAsVirtual(EntityTypeLock.Instance);

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                    .AddMapping<EntityTypeActivity, Storage.Model.Facts.Activity>()
                    .AddMapping<EntityTypeAccount, Storage.Model.Facts.Account>()
                    .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Storage.Model.Facts.BranchOfficeOrganizationUnit>()
                    .AddMapping<EntityTypeCategory, Storage.Model.Facts.Category>()
                    .AddMapping<EntityTypeCategoryFirmAddress, Storage.Model.Facts.CategoryFirmAddress>()
                    .AddMapping<EntityTypeCategoryGroup, Storage.Model.Facts.CategoryGroup>()
                    .AddMapping<EntityTypeCategoryOrganizationUnit, Storage.Model.Facts.CategoryOrganizationUnit>()
                    .AddMapping<EntityTypeClient, Storage.Model.Facts.Client>()
                    .AddMapping<EntityTypeContact, Storage.Model.Facts.Contact>()
                    .AddMapping<EntityTypeFirm, Storage.Model.Facts.Firm>()
                    .AddMapping<EntityTypeLead, Storage.Model.Facts.Lead>()
                    .AddMapping<EntityTypeFirmAddress, Storage.Model.Facts.FirmAddress>()
                    .AddMapping<EntityTypeFirmContact, Storage.Model.Facts.FirmContact>()
                    .AddMapping<EntityTypeLegalPerson, Storage.Model.Facts.LegalPerson>()
                    .AddMapping<EntityTypeOrder, Storage.Model.Facts.Order>()
                    .AddMapping<EntityTypeProject, Storage.Model.Facts.Project>()
                    .AddMapping<EntityTypeTerritory, Storage.Model.Facts.Territory>()
                    .AddMapping<EntityTypeSalesModelCategoryRestriction, Storage.Model.Facts.SalesModelCategoryRestriction>();

        // only aggregates
        private static readonly Action<EntityTypeMappingRegistryBuilder> CustomerIntelligenceTypeMap
            = builder => builder
                    .AddMapping<EntityTypeCategoryGroup, Storage.Model.CI.CategoryGroup>()
                    .AddMapping<EntityTypeClient, Storage.Model.CI.Client>()
                    .AddMapping<EntityTypeFirm, Storage.Model.CI.Firm>()
                    .AddMapping<EntityTypeProject, Storage.Model.CI.Project>()
                    .AddMapping<EntityTypeTerritory, Storage.Model.CI.Territory>()
                    .AddMapping<EntityTypeProjectStatistics, Storage.Model.Statistics.ProjectStatistics>()
                    .AddMapping<EntityTypeProjectCategoryStatistics, Storage.Model.Statistics.ProjectCategoryStatistics>()
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.ClientContact))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.ProjectCategory))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmActivity))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmLead))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmBalance))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmCategory1))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmCategory2))
                    .AddAsPersistenceOnly(typeof(Storage.Model.CI.FirmTerritory))
                    .AddAsPersistenceOnly(typeof(Storage.Model.Statistics.FirmCategory3));

        public static IEntityTypeMappingRegistry<ErmSubDomain> CreateErmContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ErmTypeMap.Invoke(builder);
            return builder.Create<ErmSubDomain>();
        }

        public static IEntityTypeMappingRegistry<CustomerIntelligenceSubDomain> CreateCustomerIntelligenceContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            CustomerIntelligenceTypeMap.Invoke(builder);
            return builder.Create<CustomerIntelligenceSubDomain>();
        }

        public static IEntityTypeMappingRegistry<FactsSubDomain> CreateFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            FactsTypeMap.Invoke(builder);
            return builder.Create<FactsSubDomain>();
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