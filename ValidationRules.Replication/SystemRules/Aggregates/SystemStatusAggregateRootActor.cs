using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.SystemRules.Aggregates
{
    public sealed class SystemStatusAggregateRootActor : AggregateRootActor<SystemStatus>
    {
        public SystemStatusAggregateRootActor(IQuery query, IEqualityComparerFactory equalityComparerFactory, IBulkRepository<SystemStatus> repository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new SystemStatusAccessor(query), repository);
        }

        public sealed class SystemStatusAccessor : DataChangesHandler<SystemStatus>, IStorageBasedDataObjectAccessor<SystemStatus>
        {
            private readonly IQuery _query;

            public SystemStatusAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AmsMessagesShouldBeNew,
                    };

            public IQueryable<SystemStatus> GetSource()
            {
                return _query.For<Facts::SystemStatus>().Select(x => new SystemStatus
                {
                    Id = x.Id,
                    SystemIsDown = x.SystemIsDown
                });
            }

            public FindSpecification<SystemStatus> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<SystemStatus>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
