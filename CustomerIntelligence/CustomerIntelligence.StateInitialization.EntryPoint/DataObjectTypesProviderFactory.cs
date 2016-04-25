using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;

using CategoryGroup = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryGroup;
using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;
using DataObjectTypesProvider = NuClear.StateInitialization.Core.DataObjectTypesProvider;
using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;
using Project = NuClear.CustomerIntelligence.Storage.Model.Facts.Project;
using Territory = NuClear.CustomerIntelligence.Storage.Model.Facts.Territory;

namespace NuClear.CustomerIntelligence.StateInitialization.Host
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
                            typeof(Storage.Model.CI.Firm),
                            typeof(FirmActivity),
                            typeof(FirmBalance),
                            typeof(FirmCategory1),
                            typeof(FirmCategory2),
                            typeof(FirmTerritory),
                            typeof(Storage.Model.CI.Client),
                            typeof(ClientContact),
                            typeof(ProjectCategory),
                            typeof(Storage.Model.CI.Territory),
                            typeof(Storage.Model.CI.CategoryGroup)
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}