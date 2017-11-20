using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public static partial class ErmDataLoader
    {
        public static void Load(long orderId, DataConnection query, IStore store)
        {
            var checkOrderIds = new[] { orderId };

            //
            var order = query.GetTable<Order>()
                             .Where(x => checkOrderIds.Contains(x.Id))
                             .Execute()
                             .Single();
            store.Add(order);

            LoadReleaseWithdrawals(query, order, store);
            LoadAccount(query, order, store);

            var bargainIds = new[] { order.BargainId };
            var dealIds = new[] { order.DealId };
            var firmIds = new[] { order.FirmId };
            var branchOfficeOrganizationUnitIds = new[] { order.BranchOfficeOrganizationUnitId };
            var legalPersonIds = new[] { order.LegalPersonId };

            var unlimitedOrder = query.GetTable<UnlimitedOrder>()
                                      .Where(x => checkOrderIds.Contains(x.OrderId))
                                      .Execute();
            store.AddRange(unlimitedOrder);

            var orderFiles = query.GetTable<OrderFile>()
                                  .Where(x => checkOrderIds.Contains(x.OrderId))
                                  .Execute();
            store.AddRange(orderFiles);

            var bills = query.GetTable<Bill>()
                             .Where(x => checkOrderIds.Contains(x.OrderId))
                             .Execute();
            store.AddRange(bills);

            var bargains = query.GetTable<Bargain>()
                                .Where(x => bargainIds.Contains(x.Id))
                                .Execute();
            store.AddRange(bargains);

            var bargainFiles = query.GetTable<BargainFile>()
                                    .Where(x => bargainIds.Contains(x.BargainId))
                                    .Execute();
            store.AddRange(bargainFiles);

            var deals = query.GetTable<Deal>()
                             .Where(x => dealIds.Contains(x.Id))
                             .Execute();
            store.AddRange(deals);

            //
            var branchOfficeOrganizationUnits = query.GetTable<BranchOfficeOrganizationUnit>()
                                                     .Where(x => branchOfficeOrganizationUnitIds.Contains(x.Id))
                                                     .Execute();
            store.AddRange(branchOfficeOrganizationUnits);
            var branchOfficeIds = branchOfficeOrganizationUnits.Select(x => x.BranchOfficeId).ToList();

            var branchOffices = query.GetTable<BranchOffice>()
                                     .Where(x => branchOfficeIds.Contains(x.Id))
                                     .Execute();
            store.AddRange(branchOffices);

            var legalPersons = query.GetTable<LegalPerson>()
                                    .Where(x => legalPersonIds.Contains(x.Id))
                                    .Execute();
            store.AddRange(legalPersons);

            var legalPersonProfiles = query.GetTable<LegalPersonProfile>()
                                           .Where(x => legalPersonIds.Contains(x.LegalPersonId))
                                           .Execute();
            store.AddRange(legalPersonProfiles);

            //
            var orders = query.GetTable<Order>()
                              .Where(op => op.IsActive && !op.IsDeleted)
                              .Where(x => firmIds.Contains(x.FirmId))
                              .Where(x => new[] { 1, 2, 4, 5 }.Contains(x.WorkflowStepId))
                              .Where(x => x.BeginDistributionDate < order.EndDistributionDatePlan && order.BeginDistributionDate < x.EndDistributionDatePlan)
                              .Execute();
            store.AddRange(orders);
            var orderIds = orders.Select(x => x.Id).ToList();

            var orderPositions = query.GetTable<OrderPosition>()
                                      .Where(op => op.IsActive && !op.IsDeleted)
                                      .Where(op => orderIds.Contains(op.OrderId))
                                      .Execute();
            store.AddRange(orderPositions);
            var orderPositionIds = orderPositions.Select(x => x.Id).ToList();
            var usedPricePositionIds = orderPositions.Select(x => x.PricePositionId).Distinct().ToList();

            var opas = query.GetTable<OrderPositionAdvertisement>()
                            .Where(op => orderPositionIds.Contains(op.OrderPositionId))
                            .Execute();
            store.AddRange(opas);
            var themeIds = opas.Where(x => x.ThemeId.HasValue).Select(x => x.ThemeId.Value).ToList();
            var categoryIds = opas.Where(x => x.CategoryId.HasValue).Select(x => x.CategoryId.Value).ToList();
            var adverisementIds = opas.Where(x => x.AdvertisementId.HasValue).Select(x => x.AdvertisementId.Value).ToList();
            var firmAddressIds = opas.Where(x => x.FirmAddressId.HasValue).Select(x => x.FirmAddressId.Value).ToList(); // список привязанных адресов из-за ЗМК может превышать список адресов фирмы

            var costs = query.GetTable<OrderPositionCostPerClick>()
                             .Where(x => orderPositionIds.Contains(x.OrderPositionId))
                             .Execute(); // Можно ужесточить: только проверямый заказ, только актуальные ставки
            store.AddRange(costs);

            //
            var themes = query.GetTable<Theme>()
                              .Where(x => themeIds.Contains(x.Id))
                              .Execute();
            store.AddRange(themes);

            var themeCategories = query.GetTable<ThemeCategory>()
                                       .Where(x => themeIds.Contains(x.ThemeId))
                                       .Where(x => categoryIds.Contains(x.CategoryId))
                                       .Execute();
            store.AddRange(themeCategories);

            var themeOrganizationUnits = query.GetTable<ThemeOrganizationUnit>()
                                              .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                                              .Where(x => themeIds.Contains(x.ThemeId))
                                              .Execute();
            store.AddRange(themeOrganizationUnits);

            var project = query.GetTable<Project>()
                               .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                               .Take(1)
                               .Execute()
                               .Single();
            store.Add(project);

            //
            var actualPrice = query.GetTable<Price>()
                                   .Where(x => !x.IsDeleted && x.IsPublished)
                                   .Where(x => x.ProjectId == project.Id && x.BeginDate <= order.BeginDistributionDate)
                                   .OrderByDescending(x => x.BeginDate)
                                   .Take(1)
                                   .Execute()
                                   .SingleOrDefault();
            if (actualPrice != null)
            {
                store.Add(actualPrice);
            }

            var usedPricePositions = query.GetTable<PricePosition>()
                                          .Where(x => usedPricePositionIds.Contains(x.Id))
                                          .Execute();
            store.AddRange(usedPricePositions);
            var monthlyUsedPrices = query.GetTable<Price>()
                                         .Where(x => x.ProjectId == project.Id && x.BeginDate >= order.BeginDistributionDate && x.BeginDate <= order.EndDistributionDatePlan)
                                         .Execute();
            store.AddRange(monthlyUsedPrices);
            var usedPriceIds = usedPricePositions.Select(x => x.PriceId).Union(actualPrice != null ? new[] { actualPrice.Id } : Array.Empty<long>()).Union(monthlyUsedPrices.Select(x => x.Id)).ToList();
            var usedPositionIds = usedPricePositions.Select(x => x.PositionId).Union(opas.Select(y => y.PositionId)).ToList();

            var positions = query.GetTable<Position>()
                                 .Where(x => usedPositionIds.Contains(x.Id))
                                 .Execute(); // Можно ограничиться проверямым заказов
            store.AddRange(positions);

            var positionChilds = query.GetTable<PositionChild>()
                                      .Where(x => usedPositionIds.Contains(x.MasterPositionId) || usedPositionIds.Contains(x.ChildPositionId))
                                      .Execute();
            store.AddRange(positionChilds);

            // Нужно ли ещё от PositionChild выбрать Position?

            var usedPrices = query.GetTable<Price>()
                                  .Where(x => usedPriceIds.Contains(x.Id))
                                  .Execute();
            store.AddRange(usedPrices);

            var releaseInfos = query.GetTable<ReleaseInfo>()
                                    .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                                    .Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == 2)
                                    .Execute(); // можно только последний?
            store.AddRange(releaseInfos);

            var categories = query.GetTable<Category>()
                                  .Where(x => categoryIds.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories);
            var cat2Ids = categories.Select(x => x.ParentId);

            var categories2 = query.GetTable<Category>()
                                  .Where(x => cat2Ids.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories2);
            var cat1Ids = categories2.Select(x => x.ParentId);

            var categories1 = query.GetTable<Category>()
                                  .Where(x => cat1Ids.Contains(x.Id))
                                  .Execute();
            store.AddRange(categories1);

            var categoryOrganizationUnit =
                query.GetTable<CategoryOrganizationUnit>()
                     .Where(x => cat1Ids.Union(cat2Ids).Union(categoryIds).Contains(x.CategoryId))
                     .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                     .Execute();
            store.AddRange(categoryOrganizationUnit);

            var costPerClickCategoryRestrictions = query.GetTable<CostPerClickCategoryRestriction>()
                                                        .Where(x => x.ProjectId == project.Id)
                                                        .Where(x => categoryIds.Contains(x.CategoryId))
                                                        .Execute(); // Можно ужесточить: рубрики из свзанных заказов нам на самом деле не нужны.
            store.AddRange(costPerClickCategoryRestrictions);

            if(costPerClickCategoryRestrictions.Any())
            {
                var maxDate = costPerClickCategoryRestrictions.Max(x => x.BeginningDate);
                var nextCostPerClickCategoryRestrictions = query.GetTable<CostPerClickCategoryRestriction>()
                                                            .Where(x => x.ProjectId == project.Id)
                                                            .Where(x => x.BeginningDate > maxDate)
                                                            .Take(1)
                                                            .Execute(); // Нужно для того, чтобы понять, что имеющиеся ограчения не являются актуальными
                store.AddRange(nextCostPerClickCategoryRestrictions);
            }

            var salesModelCategoryRestrictions = query.GetTable<SalesModelCategoryRestriction>()
                                                      .Where(x => x.ProjectId == project.Id)
                                                      .Where(x => categoryIds.Contains(x.CategoryId))
                                                      .Execute(); // Можно ужесточить: рубрики из свзанных заказов нам на самом деле не нужны.
            store.AddRange(salesModelCategoryRestrictions);

            LoadAmountControlledSales(query, order, usedPriceIds, store);
            LoadAssociatedDeniedRules(query, order, usedPriceIds, store);
            LoadFirm(query, order, firmAddressIds, store);
            LoadBuyHere(query, order, store);
        }

        private static void LoadFirm(DataConnection query, Order order, IReadOnlyCollection<long> additionalFirmIds, IStore store)
        {
            var firms =
                query.GetTable<Firm>()
                     .Where(x => x.Id == order.FirmId)
                     .Execute();
            store.AddRange(firms);
            var firmIds = firms.Select(x => x.Id);

            var firmAddresses =
                query.GetTable<FirmAddress>().Where(x => firmIds.Contains(x.FirmId))
                    .Union(query.GetTable<FirmAddress>().Where(x => additionalFirmIds.Contains(x.Id)))
                    .Execute();
            store.AddRange(firmAddresses);
            var firmAddressIds = firmAddresses.Select(y => y.Id).ToList();

            var categoryFirmAddresses =
                query.GetTable<CategoryFirmAddress>()
                     .Where(x => firmAddressIds.Contains(x.FirmAddressId))
                     .Execute();
            store.AddRange(categoryFirmAddresses);
        }

        private static void LoadAccount(DataConnection query, Order order, IStore store)
        {
            var accounts =
                query.GetTable<Account>()
                     .Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                     .Execute();
            store.AddRange(accounts);
            var accountIds = accounts.Select(x => x.Id);

            var locks =
                query.GetTable<Lock>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Where(x => accountIds.Contains(x.AccountId))
                     .Execute();
            store.AddRange(locks);
            var orderIds = locks.Select(x => x.OrderId);

            var orders =
                query.GetTable<Order>()
                     .Where(x => orderIds.Contains(x.Id))
                     .Execute();
            store.AddRange(orders);
        }

        private static void LoadReleaseWithdrawals(DataConnection query, Order order, IStore store)
        {
            var orders =
                query.GetTable<Order>()
                     .Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                     .Where(x => x.BeginDistributionDate < order.EndDistributionDatePlan && order.BeginDistributionDate < x.EndDistributionDateFact)
                     .Execute();
            var orderIds = orders.Select(x => x.Id);
            store.AddRange(orders);

            var orderPositions =
                query.GetTable<OrderPosition>()
                     .Where(x => orderIds.Contains(x.OrderId))
                     .Execute();
            var orderPositionIds = orderPositions.Select(x => x.Id);
            store.AddRange(orderPositions);

            var releaseWithdrawals =
                query.GetTable<ReleaseWithdrawal>()
                     .Where(x => orderPositionIds.Contains(x.OrderPositionId))
                     .Execute();
            store.AddRange(releaseWithdrawals);
        }

        private static void LoadAmountControlledSales(DataConnection query, Order order, IReadOnlyCollection<long> priceIds, IStore store)
        {
            // Такая хитрая номенклатура, которая не прописана как регулируемая количеством, но по факту является таковой: AdvertisementCountPerCategoryShouldBeLimited
            // todo: какого чёрта?
            const int TargetCategoryCode = 38;

            var categoryCodes =
                (from orderPosition in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == order.Id)
                 from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                 from position in query.GetTable<Position>().Where(x => (x.IsControlledByAmount || x.CategoryCode == TargetCategoryCode) && x.Id == opa.PositionId)
                 select position.CategoryCode).Distinct().Execute();

            var orders =
                 from interferringOrder in query.GetTable<Order>().Where(x => x.IsActive && !x.IsDeleted && new[] { 1, 2, 4, 5 }.Contains(x.WorkflowStepId))
                                                .Where(x => x.DestOrganizationUnitId == order.DestOrganizationUnitId && x.BeginDistributionDate < order.EndDistributionDatePlan && order.BeginDistributionDate < x.EndDistributionDatePlan)
                 from orderPosition in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == interferringOrder.Id)
                 from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                 from position in query.GetTable<Position>().Where(x => (x.IsControlledByAmount || x.CategoryCode == TargetCategoryCode) && x.Id == opa.PositionId && categoryCodes.Contains(x.CategoryCode))
                 from pricePosition in query.GetTable<PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                 select new { interferringOrder, orderPosition, opa, position, pricePosition };

            store.AddRange(orders.Select(x => x.interferringOrder).Execute());
            store.AddRange(orders.Select(x => x.orderPosition).Execute());
            store.AddRange(orders.Select(x => x.opa).Execute());
            store.AddRange(orders.Select(x => x.position).Execute());
            store.AddRange(orders.Select(x => x.pricePosition).Execute()); // Нужны для PriceCOntext.Order.OrderPostion

            // Для вычисления названий NomeclatureCategory
            var positions =
                from pricePosition in query.GetTable<PricePosition>().Where(x => priceIds.Contains(x.PriceId))
                from position in query.GetTable<Position>().Where(x => x.Id == pricePosition.PositionId).Where(x => categoryCodes.Contains(x.CategoryCode))
                select new { pricePosition, position };

            store.AddRange(positions.Select(x => x.pricePosition).Execute());
            store.AddRange(positions.Select(x => x.position).Execute());
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

            var ruleset = query.GetTable<Ruleset>()
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

