using System.Collections.Generic;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class FirmCategoryStatisticsActor : IMemoryBasedFactActor<FirmCategoryStatistics>
    {
        public FindSpecification<FirmCategoryStatistics> GetDataObjectsFindSpecification(ICommand command)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<FirmCategoryStatistics> GetDataObjects(ICommand command)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<FirmCategoryStatistics> dataObjects)
        {
            throw new System.NotImplementedException();
        }
    }
}