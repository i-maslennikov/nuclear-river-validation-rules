using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            ITracer tracer,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.OrderTheme> orderThemeBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query, tracer), bulkRepository,
               HasValueObject(new OrderThemeAccessor(query), orderThemeBulkRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;
            private readonly ITracer _tracer;

            public OrderAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
                _tracer = null;
            }

            public OrderAccessor(IQuery query, ITracer tracer) : base(CreateInvalidator())
            {
                _query = query;
                _tracer = tracer;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DefaultThemeMustHaveOnlySelfAds,
                        MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                        MessageTypeCode.ThemePeriodMustContainOrderPeriod,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                   select new Order
                   {
                       Id = order.Id,
                       BeginDistributionDate = order.BeginDistribution,
                       EndDistributionDateFact = order.EndDistributionFact,
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

                _tracer.Info("ThemeRules Order aggregateIds: " + string.Join(", ", aggregateIds));

                return new FindSpecification<Order>(x => aggregateIds.Contains(x.Id));
            }

            public override IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Order> dataObjects)
            {
                var ids = dataObjects.Select(x => x.Id);
                _tracer.Info("ThemeRules Order HandleCreates: " + string.Join(", ", ids));
                return base.HandleCreates(dataObjects);
            }

            public override IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Order> dataObjects)
            {
                var ids = dataObjects.Select(x => x.Id);
                _tracer.Info("ThemeRules Order HandleUpdates: " + string.Join(", ", ids));
                return base.HandleUpdates(dataObjects);
            }

            public override IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Order> dataObjects)
            {
                var ids = dataObjects.Select(x => x.Id);
                _tracer.Info("ThemeRules Order HandleDeletes: " + string.Join(", ", ids));
                return base.HandleDeletes(dataObjects);
            }
        }

        public sealed class OrderThemeAccessor : DataChangesHandler<Order.OrderTheme>, IStorageBasedDataObjectAccessor<Order.OrderTheme>
        {
            private readonly IQuery _query;

            public OrderThemeAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DefaultThemeMustHaveOnlySelfAds,
                        MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                        MessageTypeCode.ThemePeriodMustContainOrderPeriod,
                    };

            public IQueryable<Order.OrderTheme> GetSource()
            {
                var orderThemes = (from order in _query.For<Facts::Order>()
                                   from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.ThemeId != null).Where(x => x.OrderPositionId == op.Id)
                                   select new Order.OrderTheme
                                       {
                                           OrderId = order.Id,
                                           ThemeId = opa.ThemeId.Value
                                       }).Distinct();

                return orderThemes;
            }

            public FindSpecification<Order.OrderTheme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.OrderTheme>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
