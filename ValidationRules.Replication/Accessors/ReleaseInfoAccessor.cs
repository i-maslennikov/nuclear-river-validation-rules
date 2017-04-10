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
    public sealed class ReleaseInfoAccessor : IStorageBasedDataObjectAccessor<ReleaseInfo>, IDataChangesHandler<ReleaseInfo>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public ReleaseInfoAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<ReleaseInfo> GetSource() => _query
            .For<Erm::ReleaseInfo>()
            .Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == Erm::ReleaseInfo.Success)
            .Select(x => new ReleaseInfo
            {
                Id = x.Id,
                OrganizationUnitId = x.OrganizationUnitId,
                PeriodEndDate = x.PeriodEndDate + OneSecond,
            });

        public FindSpecification<ReleaseInfo> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<ReleaseInfo>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<ReleaseInfo> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ReleaseInfo> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ReleaseInfo> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ReleaseInfo> dataObjects)
        {
            var organizationUnitIds = dataObjects.Select(x => x.OrganizationUnitId);

            var orderIds =
                from project in _query.For<Project>().Where(project => organizationUnitIds.Contains(project.OrganizationUnitId))
                select project.Id;

            return new EventCollectionHelper<ReleaseInfo> { { typeof(Project), orderIds } };
        }
    }
}