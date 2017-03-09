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
            IBulkRepository<Firm.FirmAssociatedPosition> firmAssociatedPositionRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new FirmAccessor(query), firmRepository,
                HasValueObject(new FirmPositionAccessor(query), firmPositionRepository),
                HasValueObject(new FirmAssociatedPositionAccessor(query), firmAssociatedPositionRepository));
        }

        public sealed class FirmAccessor : DataChangesHandler<Firm>, IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query) : base(CreateInvalidator())
            {

                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                    };

            public IQueryable<Firm> GetSource()
                => from firm in _query.For<Facts::Firm>()
                   where _query.For<Facts.Order>().Any(x => x.FirmId == firm.Id)
                   select new Firm { Id = firm.Id };

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

            public FirmPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                    };

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
                    from position in _query.For<Facts::OrderItem>()
                    from order in _query.For<Facts::Order>().Where(x => x.Id == position.OrderId)
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
            private const int RulesetRuleTypeAssociated = 1;

            private readonly IQuery _query;

            public FirmAssociatedPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                    };

            public IQueryable<Firm.FirmAssociatedPosition> GetSource()
            {
                var associatedByPrice =
                    from item in _query.For<Facts::OrderItem>()
                    from apg in _query.For<Facts::AssociatedPositionsGroup>().Where(x => x.PricePositionId == item.PricePositionId)
                    from ap in _query.For<Facts::AssociatedPosition>().Where(x => x.AssociatedPositionsGroupId == apg.Id)
                    select new
                        {
                            item.OrderId,
                            item.OrderPositionId,
                            item.PackagePositionId,
                            item.ItemPositionId,

                            PrincipalPositionId = ap.PositionId,
                            ap.ObjectBindingType,

                            Source = PositionSources.Price,
                        };

                var associatedByRuleset =
                    from item in _query.For<Facts::OrderItem>()
                    from rule in _query.For<Facts::RulesetRule>().Where(x => x.RuleType == RulesetRuleTypeAssociated)
                                       .Where(x => x.DependentPositionId == item.ItemPositionId)
                    select new
                        {
                            item.OrderId,
                            item.OrderPositionId,
                            item.PackagePositionId,
                            item.ItemPositionId,

                            rule.PrincipalPositionId,
                            rule.ObjectBindingType,

                            Source = PositionSources.Ruleset,
                        };

                var associateds =
                    from associated in associatedByPrice.Union(associatedByRuleset)
                    from order in _query.For<Facts::Order>().Where(x => x.Id == associated.OrderId)
                    select new Firm.FirmAssociatedPosition
                        {
                            FirmId = order.FirmId,
                            OrderId = associated.OrderId,
                            OrderPositionId = associated.OrderPositionId,
                            PackagePositionId = associated.PackagePositionId,
                            ItemPositionId = associated.ItemPositionId,

                            PrincipalPositionId = associated.PrincipalPositionId,
                            BindingType = associated.ObjectBindingType,

                            Source = associated.Source,
                        };

                return associateds;
            }

            public FindSpecification<Firm.FirmAssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.FirmAssociatedPosition>(x => aggregateIds.Contains(x.FirmId));
            }
        }
    }
}
