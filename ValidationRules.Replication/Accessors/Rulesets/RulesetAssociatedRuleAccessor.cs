using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors.Rulesets
{
    public sealed class RulesetAssociatedRuleAccessor : IMemoryBasedDataObjectAccessor<Ruleset.AssociatedRule>, IDataChangesHandler<Ruleset.AssociatedRule>
    {
        private readonly IQuery _query;

        public RulesetAssociatedRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset.AssociatedRule> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            return dtos.SelectMany(ruleset => ruleset.AssociatedRules
                                                     .Select(rule => new Ruleset.AssociatedRule
                                                         {
                                                             RulesetId = ruleset.Id,
                                                             PrincipalNomenclatureId = rule.NomeclatureId,
                                                             AssociatedNomenclatureId = rule.AssociatedNomenclatureId,
                                                             ConsideringBindingObject = rule.ConsideringBindingObject
                                                         }))
                       .ToList();
        }

        public FindSpecification<Ruleset.AssociatedRule> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset.AssociatedRule>(x => ids.Contains(x.RulesetId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
    }
}
