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
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class FirmAggregateRootActor : EntityActorBase<Firm>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Firm.AdvantageousPurchasePositionDistributionPeriod> _advantageousPurchasePositionDistributionPeriodRepository;

        public FirmAggregateRootActor(
            IQuery query,
            IBulkRepository<Firm> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Firm.AdvantageousPurchasePositionDistributionPeriod> advantageousPurchasePositionDistributionPeriodRepository)
            : base(query, bulkRepository, equalityComparerFactory, new FirmAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _advantageousPurchasePositionDistributionPeriodRepository = advantageousPurchasePositionDistributionPeriodRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Firm.AdvantageousPurchasePositionDistributionPeriod>(_query, _advantageousPurchasePositionDistributionPeriodRepository, _equalityComparerFactory, new AdvantageousPurchasePositionDistributionPeriodAccessor(_query)),
                };

        public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Firm> GetSource()
                => from firm in _query.For<Facts::Firm>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == firm.OrganizationUnitId)
                   select new Firm
                       {
                           Id = firm.Id,
                           ProjectId = project.Id,
                           Name = firm.Name,
                       };

            public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Firm>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AdvantageousPurchasePositionDistributionPeriodAccessor : IStorageBasedDataObjectAccessor<Firm.AdvantageousPurchasePositionDistributionPeriod>
        {
            private const long AdvantageousPurchaseWith2Gis = 14; // Выгодные покупки с 2ГИС
            private const long SelfAdvertisementOnlyOnPc = 287; // Самореклама только для ПК
            private const long SpecialCategoryId = 18599; // Выгодные покупки с 2ГИС.
            private const long PlatformDesktop = 1;

            private readonly IQuery _query;

            public AdvantageousPurchasePositionDistributionPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Firm.AdvantageousPurchasePositionDistributionPeriod> GetSource()
            {
                var firmsWithCategory =
                    from firmAddressCategory in _query.For<Facts::FirmAddressCategory>().Where(x => x.CategoryId == SpecialCategoryId)
                    from firmAddress in _query.For<Facts::FirmAddress>().Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment).Where(x => x.Id == firmAddressCategory.FirmAddressId)
                    from firm in _query.For<Facts::Firm>().Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment).Where(x => x.Id == firmAddress.FirmId)
                    select firm;

                var periodsForAllFirms =
                    from firm in _query.For<Facts::Firm>()
                    select new { FirmId = firm.Id, Begin = DateTime.MinValue, End = DateTime.MaxValue, Has = false, Scope = Scope.ApprovedScope };

                var specialPositions = _query.For<Facts::Position>().Where(x => !x.IsDeleted).Select(x => new
                {
                    x.Id,
                    IsAdvantageousPurchaseOnPc = x.CategoryCode == AdvantageousPurchaseWith2Gis && x.Platform == PlatformDesktop,
                    IsSelfAdvertisementOnPc = x.CategoryCode == SelfAdvertisementOnlyOnPc,
                });

                var periodsForAllOrders =
                    from order in _query.For<Facts::Order>()
                    let has = (from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                               from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                               from specialPosition in specialPositions.Where(x => x.Id == orderPositionAdvertisement.PositionId)
                               select specialPosition).Any(x => x.IsAdvantageousPurchaseOnPc || x.IsSelfAdvertisementOnPc)
                    let scope = Scope.Compute(order.WorkflowStep, order.Id)
                    select new { order.FirmId, Begin = order.BeginDistribution, End = order.EndDistributionFact, Has = has, Scope = scope };

                var periodsForFirmsWithCategory =
                    from firm in firmsWithCategory
                    from period in periodsForAllOrders.Union(periodsForAllFirms).Where(x => x.FirmId == firm.Id)
                    select new Firm.AdvantageousPurchasePositionDistributionPeriod
                        {
                            FirmId = firm.Id,
                            Scope = period.Scope,
                            Begin = period.Begin,
                            End = period.End,
                            HasPosition = period.Has,
                        };

                return periodsForFirmsWithCategory;
            }

            public FindSpecification<Firm.AdvantageousPurchasePositionDistributionPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Firm.AdvantageousPurchasePositionDistributionPeriod>(x => aggregateIds.Contains(x.FirmId));
            }
        }
    }
}

