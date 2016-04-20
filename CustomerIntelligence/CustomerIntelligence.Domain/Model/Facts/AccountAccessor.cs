using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class AccountAccessor : IStorageBasedDataObjectAccessor<Account>, IDataChangesHandler<Account>
    {
        private readonly IQuery _query;

        public AccountAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Account> GetSource() => Specs.Map.Erm.ToFacts.Accounts.Map(_query);

        public FindSpecification<Account> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<Account>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Account> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Category>(x => ids.Contains(x.Id));

            return Specs.Map.Facts.ToFirmAggregate.ByCategory(specification)
                        .Map(_query)
                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                        .ToArray();
        }
    }
}