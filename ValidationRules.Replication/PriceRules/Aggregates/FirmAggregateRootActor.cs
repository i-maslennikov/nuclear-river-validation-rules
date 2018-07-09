using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class FirmAggregateRootActor : AggregateRootActor<Firm>
    {
        public FirmAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Firm> firmRepository,
            IBulkRepository<Firm.FirmPosition> firmPositionRepository,
            IBulkRepository<Firm.FirmAssociatedPosition> firmAssociatedPositionRepository,
            IBulkRepository<Firm.FirmDeniedPosition> firmDeniedPositionRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new FirmAccessor(query), firmRepository,
                HasValueObject(new FirmPositionAccessor(query), firmPositionRepository),
                HasValueObject(new FirmAssociatedPositionAccessor(query), firmAssociatedPositionRepository),
                HasValueObject(new FirmDeniedPositionAccessor(query), firmDeniedPositionRepository));
        }

        public sealed class FirmAccessor : DataChangesHandler<Firm>, IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator();

            public IQueryable<Firm> GetSource()
                => _query.For<Facts.Order>().Select(order => new Firm { Id = order.FirmId }).Distinct();

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

        public sealed class FirmPositionAccessor : DataChangesHandler<Firm.FirmPosition>, IStorageBasedDataObjectAccessor<Firm.FirmPosition>
        {
            private readonly IQuery _query;

            public FirmPositionAccessor(IQuery query) : base(CreateInvalidator(x => GetRelatedOrders(x, query)))
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator(Func<IReadOnlyCollection<Firm.FirmPosition>, IReadOnlyCollection<long>> func)
                => new RuleInvalidator
                    {
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipal, func },
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject, func },
                        { MessageTypeCode.FirmPositionMustNotHaveDeniedPositions, func },
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject, func },
                        MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone,
                    };

            private static IReadOnlyCollection<long> GetRelatedOrders(IReadOnlyCollection<Firm.FirmPosition> arg, IQuery query)
                => GetRelatedOrders(arg.Select(x => x.FirmId), query);

            private static IReadOnlyCollection<long> GetRelatedOrders(IEnumerable<long> firmIds, IQuery query)
                => query.For<Firm.FirmPosition>().Where(x => firmIds.Contains(x.FirmId)).Select(x => x.OrderId).Distinct().ToArray();

            public IQueryable<Firm.FirmPosition> GetSource()
            {
                var dates =
                    _query.For<Facts::Order>().Select(x => new { FirmId = x.FirmId, Date = x.BeginDistribution })
                          .Union(_query.For<Facts::Order>().Select(x => new { FirmId = x.FirmId, Date = x.EndDistributionPlan }))
                          .Union(_query.For<Facts::Order>().Select(x => new { FirmId = x.FirmId, Date = x.EndDistributionFact }))
                          .Distinct();

                var periods =
                    from begin in dates
                    let end = dates.Where(x => x.FirmId == begin.FirmId && x.Date > begin.Date).Min(x => (DateTime?)x.Date)
                    where end.HasValue
                    select new { FirmId = begin.FirmId, Begin = begin.Date, End = end.Value };

                var principals =
                    from order in _query.For<Facts::Order>()
                    from position in _query.For<Facts::OrderItem>().Where(x => order.Id == x.OrderId)
                    from period in periods.Where(x => x.FirmId == order.FirmId && x.Begin >= order.BeginDistribution && x.End <= order.EndDistributionPlan)
                    from category in _query.For<Facts::Category>().Where(x => x.Id == position.CategoryId).DefaultIfEmpty()
                    select new Firm.FirmPosition
                        {
                            FirmId = order.FirmId,
                            OrderId = position.OrderId,
                            OrderPositionId = position.OrderPositionId,
                            PackagePositionId = position.PackagePositionId,
                            ItemPositionId = position.ItemPositionId,

                            HasNoBinding = position.CategoryId == null && position.FirmAddressId == null,
                            Category1Id = category.L1Id,
                            Category3Id = category.L3Id,
                            FirmAddressId = position.FirmAddressId,

                            Scope = order.EndDistributionFact > period.Begin ? Scope.Compute(order.WorkflowStep, order.Id) : order.Id,
                            Begin = period.Begin,
                            End = period.End,
                        };

                return principals.Distinct();
            }

            public FindSpecification<Firm.FirmPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.FirmPosition>(x => aggregateIds.Contains(x.FirmId));
            }
        }

        public sealed class FirmAssociatedPositionAccessor : DataChangesHandler<Firm.FirmAssociatedPosition>, IStorageBasedDataObjectAccessor<Firm.FirmAssociatedPosition>
        {
            private readonly IQuery _query;

            public FirmAssociatedPositionAccessor(IQuery query) : base(CreateInvalidator(x => GetRelatedOrders(x, query)))
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator(Func<IReadOnlyCollection<Firm.FirmAssociatedPosition>, IReadOnlyCollection<long>> func)
                => new RuleInvalidator
                    {
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipal, func },
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject, func },
                        { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject, func },
                        MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone,
                    };

            private static IReadOnlyCollection<long> GetRelatedOrders(IReadOnlyCollection<Firm.FirmAssociatedPosition> arg, IQuery query)
                => GetRelatedOrders(arg.Select(x => x.FirmId), query);

            private static IReadOnlyCollection<long> GetRelatedOrders(IEnumerable<long> firmIds, IQuery query)
                => query.For<Firm.FirmPosition>().Where(x => firmIds.Contains(x.FirmId)).Select(x => x.OrderId).Distinct().ToArray();

            public IQueryable<Firm.FirmAssociatedPosition> GetSource()
            {
                const int BindingTypeMatch = 1;
                const int BindingTypeNoDependency = 2;

                var result =
                    from order in _query.For<Facts::Order>()
                    from item in _query.For<Facts::OrderItem>().Where(x => x.OrderId == order.Id)
                    from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                    from rp in _query.For<Facts::Ruleset.RulesetProject>().Where(x => x.ProjectId == project.Id)
                    from ruleset in _query.For<Facts::Ruleset>().Where(x => x.Id == rp.RulesetId && x.BeginDate <= order.BeginDistribution && order.BeginDistribution < x.EndDate)
                    from rule in _query.For<Facts::Ruleset.AssociatedRule>().Where(x => x.RulesetId == ruleset.Id && x.AssociatedNomenclatureId == item.ItemPositionId)
                    select new Firm.FirmAssociatedPosition
                        {
                            FirmId = order.FirmId,
                            OrderId = order.Id,

                            ItemPositionId = item.ItemPositionId,
                            OrderPositionId = item.OrderPositionId,
                            PackagePositionId = item.PackagePositionId,

                            PrincipalPositionId = rule.PrincipalNomenclatureId,
                            BindingType = rule.ConsideringBindingObject ? BindingTypeMatch : BindingTypeNoDependency
                        };

                return result;
            }

            public FindSpecification<Firm.FirmAssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.FirmAssociatedPosition>(x => aggregateIds.Contains(x.FirmId));
            }
        }

        public sealed class FirmDeniedPositionAccessor : DataChangesHandler<Firm.FirmDeniedPosition>, IStorageBasedDataObjectAccessor<Firm.FirmDeniedPosition>
        {
            private readonly IQuery _query;

            public FirmDeniedPositionAccessor(IQuery query) : base(CreateInvalidator(x => GetRelatedOrders(x, query)))
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator(Func<IReadOnlyCollection<Firm.FirmDeniedPosition>, IReadOnlyCollection<long>> func)
                => new RuleInvalidator
                    {
                        { MessageTypeCode.FirmPositionMustNotHaveDeniedPositions, func },
                    };

            private static IReadOnlyCollection<long> GetRelatedOrders(IReadOnlyCollection<Firm.FirmDeniedPosition> arg, IQuery query)
                => GetRelatedOrders(arg.Select(x => x.FirmId), query);

            private static IReadOnlyCollection<long> GetRelatedOrders(IEnumerable<long> firmIds, IQuery query)
                => query.For<Firm.FirmPosition>().Where(x => firmIds.Contains(x.FirmId)).Select(x => x.OrderId).Distinct().ToArray();

            public IQueryable<Firm.FirmDeniedPosition> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    from item in _query.For<Facts::OrderItem>().Where(x => x.OrderId == order.Id)
                    from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                    from rp in _query.For<Facts::Ruleset.RulesetProject>().Where(x => x.ProjectId == project.Id)
                    from ruleset in _query.For<Facts::Ruleset>().Where(x => x.Id == rp.RulesetId && x.BeginDate <= order.BeginDistribution && order.BeginDistribution < x.EndDate)
                    from rule in _query.For<Facts::Ruleset.DeniedRule>().Where(x => x.RulesetId == ruleset.Id && x.NomenclatureId == item.ItemPositionId)
                    select new Firm.FirmDeniedPosition
                        {
                            FirmId = order.FirmId,
                            OrderId = order.Id,

                            OrderPositionId = item.OrderPositionId,
                            PackagePositionId = item.PackagePositionId,
                            ItemPositionId = item.ItemPositionId,

                            DeniedPositionId = rule.DeniedNomenclatureId,
                            BindingType = rule.BindingObjectStrategy
                        };

                return result;
            }

            public FindSpecification<Firm.FirmDeniedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.FirmDeniedPosition>(x => aggregateIds.Contains(x.FirmId));
            }
        }
    }
}
