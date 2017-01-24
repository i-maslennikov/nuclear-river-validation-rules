using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class CostPerClickCategoryRestrictionAccessor : IStorageBasedDataObjectAccessor<CostPerClickCategoryRestriction>, IDataChangesHandler<CostPerClickCategoryRestriction>
    {
        private readonly IQuery _query;

        public CostPerClickCategoryRestrictionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CostPerClickCategoryRestriction> GetSource() => _query
            .For<Erm::CostPerClickCategoryRestriction>()
            .Select(x => new CostPerClickCategoryRestriction
            {
                CategoryId = x.CategoryId,
                ProjectId = x.ProjectId,
                MinCostPerClick = x.MinCostPerClick,
                Begin = x.BeginningDate,
            });

        public FindSpecification<CostPerClickCategoryRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<CostPerClickCategoryRestriction>.Contains(x => x.ProjectId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        // value object для Project, ничего пересчитывать не надо
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CostPerClickCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();
    }
}