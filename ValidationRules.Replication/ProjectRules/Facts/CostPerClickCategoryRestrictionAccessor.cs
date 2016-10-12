using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class CostPerClickCategoryRestrictionAccessor : IStorageBasedDataObjectAccessor<CostPerClickCategoryRestriction>, IDataChangesHandler<CostPerClickCategoryRestriction>
    {
        private readonly IQuery _query;

        public CostPerClickCategoryRestrictionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CostPerClickCategoryRestriction> GetSource()
            => from x in _query.For<Storage.Model.Erm.CostPerClickCategoryRestriction>()
               select new CostPerClickCategoryRestriction
                   {
                       CategoryId = x.CategoryId,
                       ProjectId = x.ProjectId,
                       MinCostPerClick = x.MinCostPerClick,
                       Begin = x.BeginningDate,
                   };

        public FindSpecification<CostPerClickCategoryRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CostPerClickCategoryRestriction>(x => ids.Contains(x.ProjectId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
        {
            // ProjectId ограничения не меняется
            var ids = dataObjects.Select(x => x.ProjectId).Distinct();

            return new EventCollectionHelper { { typeof(Project), ids } };
        }
    }
}