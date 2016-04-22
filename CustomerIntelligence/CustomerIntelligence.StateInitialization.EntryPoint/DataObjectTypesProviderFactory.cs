using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Replication.Bulk.API.Commands;
using NuClear.Replication.Bulk.API.Factories;
using NuClear.Replication.Core.API.DataObjects;

using DataObjectTypesProvider = NuClear.Replication.Bulk.API.DataObjectTypesProvider;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    public class DataObjectTypesProviderFactory : IDataObjectTypesProviderFactory
    {
        public IDataObjectTypesProvider Create(ReplaceDataObjectsInBulkCommand command)
        {
            if (command.TargetStorageDescriptor.ConnectionStringName == ConnectionStringName.Facts)
            {
                return new DataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(Project),
                            typeof(Category),
                            typeof(CategoryGroup),
                            typeof(Territory),
                            typeof(Client),
                            typeof(Firm),
                            typeof(Account),
                            typeof(Activity),
                            typeof(CategoryFirmAddress),
                            typeof(CategoryOrganizationUnit),
                            typeof(Contact),
                            typeof(FirmAddress),
                            typeof(FirmContact),
                            typeof(LegalPerson),
                            typeof(Order),
                            typeof(SalesModelCategoryRestriction)
                        });
            }
            if (command.TargetStorageDescriptor.ConnectionStringName == ConnectionStringName.CustomerIntelligence)
            {
                return new DataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(Domain.Model.CI.Firm),
                            typeof(Domain.Model.CI.FirmActivity),
                            typeof(Domain.Model.CI.FirmBalance),
                            typeof(Domain.Model.CI.FirmCategory1),
                            typeof(Domain.Model.CI.FirmCategory2),
                            typeof(Domain.Model.CI.FirmTerritory),
                            typeof(Domain.Model.CI.Client),
                            typeof(Domain.Model.CI.ClientContact),
                            typeof(Domain.Model.CI.ProjectCategory),
                            typeof(Domain.Model.CI.Territory),
                            typeof(Domain.Model.CI.CategoryGroup)
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}