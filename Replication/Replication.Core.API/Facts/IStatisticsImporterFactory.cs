using System;
using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStatisticsImporterFactory
    {
        IReadOnlyCollection<IStatisticsImporter> Create(Type statisticsDtoType);
    }
}