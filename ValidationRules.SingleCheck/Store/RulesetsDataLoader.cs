using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.Storage.Model.Erm;

using Ruleset = NuClear.ValidationRules.Storage.Model.Facts.Ruleset;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public static class RulesetsDataLoader
    {
        public static void Load(Order order, DataConnection query, IStore store)
        {
            var targetRulesets = query.GetTable<Ruleset>()
                                      .Where(r => r.BeginDate <= order.BeginDistributionDate && r.EndDate > order.EndDistributionDatePlan)
                                      .Execute();

            store.AddRange(targetRulesets);

            var targetRulesetsIds = targetRulesets.Select(r => r.Id)
                                                  .ToList();

            var associatedRules = query.GetTable<Ruleset.AssociatedRule>()
                                       .Where(rule => targetRulesetsIds.Contains(rule.RulesetId))
                                       .Execute();
            store.AddRange(associatedRules);

            var deniedRules = query.GetTable<Ruleset.DeniedRule>()
                                       .Where(rule => targetRulesetsIds.Contains(rule.RulesetId))
                                       .Execute();
            store.AddRange(deniedRules);

            var quantitativeRules = query.GetTable<Ruleset.QuantitativeRule>()
                                   .Where(rule => targetRulesetsIds.Contains(rule.RulesetId))
                                   .Execute();
            store.AddRange(quantitativeRules);

            var rulesetsProjects = query.GetTable<Ruleset.RulesetProject>()
                                        .Where(rp => targetRulesetsIds.Contains(rp.RulesetId))
                                        .Execute();
            store.AddRange(rulesetsProjects);
        }

        private static void LoadAssociatedDeniedRules(DataConnection query, Order order, IReadOnlyCollection<long> priceIds, IStore store)
        {
            // Правила на запрещение требуются только по PositionId, которые есть в заказе
            // Правила на сопутствие требуются только по PositionId или AssociatedPositionId, которые есть в заказе
            var orderPositions =
                query.GetTable<OrderPosition>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Where(x => x.OrderId == order.Id)
                     .Select(x => new { x.Id, x.PricePositionId })
                     .Execute();
            var orderPositionIds = orderPositions.Select(x => x.Id).ToList();
            var pricePositionIds = orderPositions.Select(x => x.PricePositionId).Distinct().ToList();

            var positions1 =
                query.GetTable<PricePosition>()
                     .Where(x => pricePositionIds.Contains(x.Id))
                     .Select(x => x.PositionId);
            var positions2 =
                query.GetTable<OrderPositionAdvertisement>()
                     .Where(x => orderPositionIds.Contains(x.OrderPositionId))
                     .Select(x => x.PositionId);

            var positionIds = positions1.Union(positions2).Execute();

            var deniedPositions = query.GetTable<DeniedPosition>()
                                       .Where(x => priceIds.Contains(x.PriceId))
                                       .Where(x => positionIds.Contains(x.PositionId))
                                       .Execute();
            store.AddRange(deniedPositions);

            // Группы и позиции, в которых текущий заказ играет роль сопуствующего
            var associatedGroups =
                query.GetTable<AssociatedPositionsGroup>()
                     .Where(x => x.IsActive && !x.IsDeleted && pricePositionIds.Contains(x.PricePositionId))
                     .Execute();
            var associatedGroupIds = associatedGroups.Select(x => x.Id).ToList();
            store.AddRange(associatedGroups);

            var associatedPositions =
                query.GetTable<AssociatedPosition>()
                     .Where(x => x.IsActive && !x.IsDeleted && associatedGroupIds.Contains(x.AssociatedPositionsGroupId))
                     .Execute();
            store.AddRange(associatedPositions);

            // Группы и позиции, в которых текущий заказ играет роль основного
            var masterPositions =
                from g in query.GetTable<AssociatedPositionsGroup>().Where(x => x.IsActive && !x.IsDeleted)
                from p in query.GetTable<AssociatedPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.AssociatedPositionsGroupId == g.Id)
                from pp in query.GetTable<PricePosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.Id == g.PricePositionId)
                where priceIds.Contains(pp.PriceId) && positionIds.Contains(p.PositionId)
                select new { g, p };
            store.AddRange(masterPositions.Select(x => x.g).Execute());
            store.AddRange(masterPositions.Select(x => x.p).Execute());

            var ruleset = query.GetTable<Storage.Model.Erm.Ruleset>()
                               .Where(x => !x.IsDeleted && x.Priority != 0)
                               .OrderByDescending(x => x.Priority)
                               .Take(1)
                               .Execute()
                               .Single();
            store.Add(ruleset);

            var rulesetRules = query.GetTable<RulesetRule>()
                                    .Where(x => x.RulesetId == ruleset.Id)
                                    .Where(x => positionIds.Contains(x.DependentPositionId) || positionIds.Contains(x.PrincipalPositionId))
                                    .Execute();
            store.AddRange(rulesetRules);
        }
    }
}
