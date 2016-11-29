using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class FirmAddressAggregateRootActor : AggregateRootActor<FirmAddress>
    {
        public FirmAddressAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<FirmAddress> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new FirmAddressAccessor(query), bulkRepository);
        }

        public sealed class FirmAddressAccessor : DataChangesHandler<FirmAddress>, IStorageBasedDataObjectAccessor<FirmAddress>
        {
            private readonly IQuery _query;

            public FirmAddressAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                    };

            public IQueryable<FirmAddress> GetSource()
                => from address in _query.For<Facts::FirmAddress>()
                   select new FirmAddress
                   {
                       Id = address.Id,
                       Name = address.Name,
                       IsLocatedOnTheMap = address.IsLocatedOnTheMap,
                   };

            public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<FirmAddress>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}

