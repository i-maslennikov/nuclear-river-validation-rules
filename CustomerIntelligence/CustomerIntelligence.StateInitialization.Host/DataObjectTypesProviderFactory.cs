using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;

namespace NuClear.CustomerIntelligence.StateInitialization.Host
{
    public sealed class DataObjectTypesProviderFactory : IDataObjectTypesProviderFactory
    {
        public IDataObjectTypesProvider Create(ReplaceDataObjectsInBulkCommand command)
        {
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is FactsConnectionStringIdentity)
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
                            typeof(BranchOfficeOrganizationUnit),
                            typeof(CategoryFirmAddress),
                            typeof(CategoryOrganizationUnit),
                            typeof(Contact),
                            typeof(FirmAddress),
                            typeof(FirmContact),
                            typeof(Lead),
                            typeof(LegalPerson),
                            typeof(Order),
                            typeof(SalesModelCategoryRestriction)
                        });
            }
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is CustomerIntelligenceConnectionStringIdentity)
            {
                return new DataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(Storage.Model.CI.Firm),
                            typeof(Storage.Model.CI.FirmActivity),
                            typeof(Storage.Model.CI.FirmBalance),
                            typeof(Storage.Model.CI.FirmCategory1),
                            typeof(Storage.Model.CI.FirmCategory2),
                            typeof(Storage.Model.CI.FirmLead),
                            typeof(Storage.Model.CI.FirmTerritory),
                            typeof(Storage.Model.CI.Client),
                            typeof(Storage.Model.CI.ClientContact),
                            typeof(Storage.Model.CI.Project),
                            typeof(Storage.Model.CI.ProjectCategory),
                            typeof(Storage.Model.CI.Territory),
                            typeof(Storage.Model.CI.CategoryGroup),
                            typeof(Storage.Model.Statistics.ProjectStatistics),
                            typeof(Storage.Model.Statistics.ProjectCategoryStatistics),
                            typeof(Storage.Model.Statistics.FirmForecast),
                            typeof(Storage.Model.Statistics.FirmCategory3),
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}