using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class AdvertisementElementTemplateAggregateRootActor : AggregateRootActor<AdvertisementElementTemplate>
    {
        public AdvertisementElementTemplateAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<AdvertisementElementTemplate> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new AdvertisementElementTemplateAccessor(query), bulkRepository);
        }

        public sealed class AdvertisementElementTemplateAccessor : DataChangesHandler<AdvertisementElementTemplate>, IStorageBasedDataObjectAccessor<AdvertisementElementTemplate>
        {
            private readonly IQuery _query;

            public AdvertisementElementTemplateAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementElementMustPassReview,
                        MessageTypeCode.OrderMustHaveAdvertisement,
                    };

            public IQueryable<AdvertisementElementTemplate> GetSource()
                => from template in _query.For<Facts::AdvertisementElementTemplate>()
                   select new AdvertisementElementTemplate
                   {
                       Id = template.Id,
                       Name = template.Name,
                   };

            public FindSpecification<AdvertisementElementTemplate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<AdvertisementElementTemplate>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
