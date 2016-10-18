using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Order.OrderTheme> _orderThemeBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.OrderTheme> orderThemeBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _orderThemeBulkRepository = orderThemeBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.OrderTheme>(_query, _orderThemeBulkRepository, _equalityComparerFactory, new OrderThemeAccessor(_query)),
                };

        public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                   select new Order
                   {
                       Id = order.Id,
                       Number = order.Number,
                       BeginDistributionDate = order.BeginDistributionDate,
                       EndDistributionDateFact = order.EndDistributionDateFact,
                       ProjectId = project.Id,
                       IsSelfAds = order.IsSelfAds,
                   };

            public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class OrderThemeAccessor : IStorageBasedDataObjectAccessor<Order.OrderTheme>
        {
            private readonly IQuery _query;

            public OrderThemeAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.OrderTheme> GetSource()
            {
                var orderThemes = (from order in _query.For<Facts::Order>()
                                    from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == op.Id)
                                    select new Order.OrderTheme
                                    {
                                        OrderId = order.Id,
                                        ThemeId = opa.ThemeId
                                    }).Distinct();

                return orderThemes;
            }

            public FindSpecification<Order.OrderTheme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.OrderTheme>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
