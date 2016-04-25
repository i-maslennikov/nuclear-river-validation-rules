using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.DataObjects;

namespace NuClear.CustomerIntelligence.Domain
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(TCommand) == typeof(SyncDataObjectCommand))
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
                        typeof(SalesModelCategoryRestriction)
                    };
            }

            if (typeof(TCommand) == typeof(ReplaceFirmCategoryForecastCommand))
            {
                return new List<Type> { typeof(FirmCategoryForecast) };
            }

            if (typeof(TCommand) == typeof(ReplaceFirmForecastCommand))
            {
                return new List<Type> { typeof(FirmForecast) };
            }

            if (typeof(TCommand) == typeof(FirmCategoryStatistics))
            {
                return new List<Type> { typeof(FirmCategoryForecast) };
            }

            if (typeof(TCommand) == typeof(ReplaceRubricPopularityCommand))
            {
                return new List<Type> { typeof(ProjectCategoryStatistics) };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}