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
using NuClear.ValidationRules.Storage.Model.Aggregates;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class RulesetAggregateRootActor : EntityActorBase<Ruleset>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<RulesetRule> _rulesetRuleBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public RulesetAggregateRootActor(
            IQuery query,
            IBulkRepository<Ruleset> bulkRepository,
            IBulkRepository<RulesetRule> rulesetRuleBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new RulesetAccessor(query))
        {
            _query = query;
            _rulesetRuleBulkRepository = rulesetRuleBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<RulesetRule>(_query, _rulesetRuleBulkRepository, _equalityComparerFactory, new RulesetRuleAccessor(_query)),
                };

        public sealed class RulesetAccessor : IStorageBasedDataObjectAccessor<Ruleset>
        {
            private readonly IQuery _query;

            public RulesetAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Ruleset> GetSource() => Specs.Map.Facts.ToAggregates.Rulesets.Map(_query);

            public FindSpecification<Ruleset> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Ruleset>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class RulesetRuleAccessor : IStorageBasedDataObjectAccessor<RulesetRule>
        {
            private readonly IQuery _query;

            public RulesetRuleAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<RulesetRule> GetSource() => Specs.Map.Facts.ToAggregates.RulesetRules.Map(_query);

            public FindSpecification<RulesetRule> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.RulesetRules(aggregateIds);
            }
        }
    }
}