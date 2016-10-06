using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Order.AddressAdvertisement> _addressAdvertisementRepository;
        private readonly IBulkRepository<Order.CategoryAdvertisement> _categoryAdvertisementRepository;
        private readonly IBulkRepository<Order.CostPerClickAdvertisement> _costPerClickAdvertisementRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.AddressAdvertisement> addressAdvertisementRepository,
            IBulkRepository<Order.CategoryAdvertisement> categoryAdvertisementRepository,
            IBulkRepository<Order.CostPerClickAdvertisement> costPerClickAdvertisementRepository)
            : base(query, bulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _addressAdvertisementRepository = addressAdvertisementRepository;
            _costPerClickAdvertisementRepository = costPerClickAdvertisementRepository;
            _categoryAdvertisementRepository = categoryAdvertisementRepository;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.AddressAdvertisement>(_query, _addressAdvertisementRepository, _equalityComparerFactory, new AddressAdvertisementAccessor(_query)),
                    new ValueObjectActor<Order.CategoryAdvertisement>(_query, _categoryAdvertisementRepository, _equalityComparerFactory, new CategoryAdvertisementAccessor(_query)),
                    new ValueObjectActor<Order.CostPerClickAdvertisement>(_query, _costPerClickAdvertisementRepository, _equalityComparerFactory, new CostPerClickAdvertisementAccessor(_query)),
                };

        public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>
        {
            private const int OrderOnRegistration = 1;

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
                           Begin = order.BeginDistribution,
                           End = order.EndDistributionPlan, // ?
                           ProjectId = project.Id,
                           IsDraft = order.WorkflowStep == OrderOnRegistration,
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

        public sealed class AddressAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.AddressAdvertisement>
        {
            private static readonly long[] ExceptionalCategoryCodes =
                {
                    11, // Рекламная ссылка
                    14, // Выгодные покупки с 2ГИС
                    26, // Комментарий к адресу
                };

            private readonly IQuery _query;

            public AddressAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.AddressAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                   from position in _query.For<Facts::Position>().Where(x => x.Id == orderPositionAdvertisement.PositionId)
                   from firmAddress in _query.For<Facts::FirmAddress>().Where(x => x.Id == orderPositionAdvertisement.FirmAddressId)
                   select new Order.AddressAdvertisement
                       {
                           AddressId = firmAddress.Id,
                           OrderId = order.Id,
                           OrderPositionId = orderPosition.Id,
                           PositionId = orderPositionAdvertisement.PositionId,
                           MustBeLocatedOnTheMap = !ExceptionalCategoryCodes.Contains(position.CategoryCode)
                       };

            public FindSpecification<Order.AddressAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.AddressAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CategoryAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.CategoryAdvertisement>
        {
            private readonly IQuery _query;

            public CategoryAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.CategoryAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                   where orderPositionAdvertisement.CategoryId.HasValue
                   select new Order.CategoryAdvertisement
                       {
                           OrderId = order.Id,
                           OrderPositionId = orderPosition.Id,
                           PositionId = orderPositionAdvertisement.PositionId,
                           CategoryId = orderPositionAdvertisement.CategoryId.Value,
                       };

            public FindSpecification<Order.CategoryAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CategoryAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CostPerClickAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.CostPerClickAdvertisement>
        {
            private readonly IQuery _query;

            public CostPerClickAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.CostPerClickAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                   from cpc in _query.For<Facts::OrderPositionCostPerClick>().Where(x => x.OrderPositionId == orderPosition.Id)
                   select new Order.CostPerClickAdvertisement
                       {
                           OrderId = order.Id,
                           OrderPositionId = orderPosition.Id,
                           PositionId = pricePosition.PositionId,
                           CategoryId = cpc.CategoryId,
                           Bid = cpc.Amount,
                       };

            public FindSpecification<Order.CostPerClickAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CostPerClickAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
