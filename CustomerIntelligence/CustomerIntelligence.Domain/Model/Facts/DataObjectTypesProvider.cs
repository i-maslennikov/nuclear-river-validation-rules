using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(TCommand) == typeof(SyncDataObjectCommand))
            {
                return new List<Type>
                    {
                        typeof(Client),
                        typeof(Firm),
                        typeof(Territory),
                        typeof(Project),
                        typeof(Category),
                        typeof(CategoryGroup),
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