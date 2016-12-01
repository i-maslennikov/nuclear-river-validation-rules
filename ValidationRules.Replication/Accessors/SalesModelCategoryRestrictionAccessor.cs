using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class SalesModelCategoryRestrictionAccessor : IStorageBasedDataObjectAccessor<SalesModelCategoryRestriction>, IDataChangesHandler<SalesModelCategoryRestriction>
    {
        private readonly IQuery _query;

        public SalesModelCategoryRestrictionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<SalesModelCategoryRestriction> GetSource() => _query
            .For<Erm::SalesModelCategoryRestriction>()
            .Select(x => new SalesModelCategoryRestriction
            {
                CategoryId = x.CategoryId,
                ProjectId = x.ProjectId,
                SalesModel = x.SalesModel,
                Begin = x.BeginningDate,
            });

        public FindSpecification<SalesModelCategoryRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<SalesModelCategoryRestriction>.Create(x => x.ProjectId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => Array.Empty<IEvent>();
    }
}