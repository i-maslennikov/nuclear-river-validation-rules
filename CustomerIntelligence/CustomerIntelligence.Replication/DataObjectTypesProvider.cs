using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.CustomerIntelligence.Replication
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(ISyncDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new List<Type>
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
                        typeof(SalesModelCategoryRestriction),
                        typeof(Lead)
                    };
            }

            if (typeof(IReplaceDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new List<Type>
                    {
                        typeof(FirmCategoryForecast),
                        typeof(FirmForecast),
                        typeof(FirmCategoryStatistics),
                        typeof(ProjectCategoryStatistics)
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}