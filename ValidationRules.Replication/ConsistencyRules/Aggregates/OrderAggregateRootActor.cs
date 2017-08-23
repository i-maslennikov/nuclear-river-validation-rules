using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.InvalidCategory> invalidCategoryRepository,
            IBulkRepository<Order.CategoryNotBelongsToAddress> categoryNotBelongsToAddress,
            IBulkRepository<Order.InvalidFirmAddress> orderInvalidFirmAddressRepository,
            IBulkRepository<Order.BargainSignedLaterThanOrder> orderBargainSignedLaterThanOrderRepository,
            IBulkRepository<Order.HasNoAnyLegalPersonProfile> orderHasNoAnyLegalPersonProfileRepository,
            IBulkRepository<Order.HasNoAnyPosition> orderHasNoAnyPositionRepository,
            IBulkRepository<Order.InactiveReference> inactiveReferenceRepository,
            IBulkRepository<Order.InvalidBeginDistributionDate> orderInvalidBeginDistributionDateRepository,
            IBulkRepository<Order.InvalidEndDistributionDate> orderInvalidEndDistributionDateRepository,
            IBulkRepository<Order.InvalidBillsTotal> orderInvalidBillsTotalRepository,
            IBulkRepository<Order.LegalPersonProfileBargainExpired> orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository,
            IBulkRepository<Order.LegalPersonProfileWarrantyExpired> orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository,
            IBulkRepository<Order.MissingBargainScan> orderMissingBargainScanRepository,
            IBulkRepository<Order.MissingBills> orderMissingBillsRepository,
            IBulkRepository<Order.MissingRequiredField> orderMissingRequiredFieldRepository,
            IBulkRepository<Order.MissingOrderScan> orderMissingOrderScanRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new InvalidCategoryAccessor(query), invalidCategoryRepository),
                HasValueObject(new CategoryNotBelongsToAddressAccessor(query), categoryNotBelongsToAddress),
                HasValueObject(new InvalidFirmAddressAccessor(query), orderInvalidFirmAddressRepository),
                HasValueObject(new OrderBargainSignedLaterThanOrderAccessor(query), orderBargainSignedLaterThanOrderRepository),
                HasValueObject(new OrderHasNoAnyLegalPersonProfileAccessor(query), orderHasNoAnyLegalPersonProfileRepository),
                HasValueObject(new OrderHasNoAnyPositionAccessor(query), orderHasNoAnyPositionRepository),
                HasValueObject(new InactiveReferenceAccessor(query), inactiveReferenceRepository),
                HasValueObject(new OrderInvalidBeginDistributionDateAccessor(query), orderInvalidBeginDistributionDateRepository),
                HasValueObject(new OrderInvalidEndDistributionDateAccessor(query), orderInvalidEndDistributionDateRepository),
                HasValueObject(new OrderInvalidBillsTotalAccessor(query), orderInvalidBillsTotalRepository),
                HasValueObject(new LegalPersonProfileBargainExpiredAccessor(query), orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository),
                HasValueObject(new LegalPersonProfileWarrantyExpiredAccessor(query), orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository),
                HasValueObject(new OrderMissingBargainScanAccessor(query), orderMissingBargainScanRepository),
                HasValueObject(new OrderMissingBillsAccessor(query), orderMissingBillsRepository),
                HasValueObject(new MissingRequiredFieldAccessor(query), orderMissingRequiredFieldRepository),
                HasValueObject(new OrderMissingOrderScanAccessor(query), orderMissingOrderScanRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.BargainScanShouldPresent,
                        MessageTypeCode.BillsShouldBeCreated,
                        MessageTypeCode.BillsSumShouldMatchOrder,
                        MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired,
                        MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired,
                        MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile,
                        MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm,
                        MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid,
                        MessageTypeCode.LinkedCategoryShouldBeActive,
                        MessageTypeCode.LinkedCategoryShouldBelongToFirm,
                        MessageTypeCode.LinkedFirmAddressShouldBeValid,
                        MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth,
                        MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth,
                        MessageTypeCode.OrderMustHaveActiveDeal,
                        MessageTypeCode.OrderMustHaveActiveLegalEntities,
                        MessageTypeCode.OrderRequiredFieldsShouldBeSpecified,
                        MessageTypeCode.OrderScanShouldPresent,
                        MessageTypeCode.OrderShouldHaveAtLeastOnePosition,
                        MessageTypeCode.OrderShouldNotBeSignedBeforeBargain,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   select new Order
                       {
                           Id = order.Id,
                           BeginDistribution = order.BeginDistribution,
                           EndDistributionFact = order.EndDistributionFact,
                           EndDistributionPlan = order.EndDistributionPlan,
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

        public sealed class InvalidFirmAddressAccessor : DataChangesHandler<Order.InvalidFirmAddress>, IStorageBasedDataObjectAccessor<Order.InvalidFirmAddress>
        {
            private readonly IQuery _query;

            public InvalidFirmAddressAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LinkedFirmAddressShouldBeValid,
                    };

            public IQueryable<Order.InvalidFirmAddress> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                   from position in _query.For<Facts::Position>().Where(x => x.Id == opa.PositionId)
                   from address in _query.For<Facts::FirmAddress>().Where(x => x.Id == opa.FirmAddressId)
                   let isFirmMismatchAllowed = Facts.Position.CategoryCodesAllowFirmMismatch.Contains(position.CategoryCode) && position.BindingObjectType == Facts.Position.BindingObjectTypeAddressMultiple
                   let state = address.FirmId != order.FirmId && !isFirmMismatchAllowed ? InvalidFirmAddressState.NotBelongToFirm
                                : address.IsDeleted ? InvalidFirmAddressState.Deleted
                                : !address.IsActive ? InvalidFirmAddressState.NotActive
                                : address.IsClosedForAscertainment ? InvalidFirmAddressState.ClosedForAscertainment
                                : InvalidFirmAddressState.NotSet
                   where state != InvalidFirmAddressState.NotSet // todo: интересно было бы глянуть на сгенерированный sql
                   select new Order.InvalidFirmAddress
                       {
                           OrderId = order.Id,
                           FirmAddressId = address.Id,
                           OrderPositionId = orderPosition.Id,
                           PositionId = opa.PositionId,
                           State = state,
                       };

            public FindSpecification<Order.InvalidFirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidFirmAddress>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CategoryNotBelongsToAddressAccessor : DataChangesHandler<Order.CategoryNotBelongsToAddress>, IStorageBasedDataObjectAccessor<Order.CategoryNotBelongsToAddress>
        {
            private readonly IQuery _query;

            public CategoryNotBelongsToAddressAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid,
                    };

            public IQueryable<Order.CategoryNotBelongsToAddress> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue).Where(x => x.OrderPositionId == orderPosition.Id)
                   from address in _query.For<Facts::FirmAddress>().Where(x => x.IsActive && !x.IsClosedForAscertainment && !x.IsDeleted).Where(x => x.Id == opa.FirmAddressId)
                   from cfa in _query.For<Facts::FirmAddressCategory>().Where(x => x.FirmAddressId == address.Id && x.CategoryId == opa.CategoryId.Value).DefaultIfEmpty()
                   where cfa == null
                   select new Order.CategoryNotBelongsToAddress
                   {
                       OrderId = order.Id,
                       FirmAddressId = address.Id,
                       CategoryId = opa.CategoryId.Value,
                       OrderPositionId = orderPosition.Id,
                       PositionId = opa.PositionId,
                   };

            public FindSpecification<Order.CategoryNotBelongsToAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CategoryNotBelongsToAddress>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class InvalidCategoryAccessor : DataChangesHandler<Order.InvalidCategory>, IStorageBasedDataObjectAccessor<Order.InvalidCategory>
        {
            private readonly IQuery _query;

            public InvalidCategoryAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm,
                        MessageTypeCode.LinkedCategoryShouldBeActive,
                        MessageTypeCode.LinkedCategoryShouldBelongToFirm,
                    };

            public IQueryable<Order.InvalidCategory> GetSource()
            {
                var result = 
                    from order in _query.For<Facts::Order>()
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue).Where(x => x.OrderPositionId == orderPosition.Id)
                    from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId)
                    from position in _query.For<Facts::Position>().Where(x => !x.IsDeleted).Where(x => x.Id == opa.PositionId)
                    let categoryBelongToFirm = _query.For<Facts::FirmAddress>()
                                                     .Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment)
                                                     .Where(x => x.FirmId == order.FirmId)
                                                     .SelectMany(fa => _query.For<Facts::FirmAddressCategory>().Where(cfa => cfa.FirmAddressId == fa.Id))
                                                     .Any(x => x.CategoryId == opa.CategoryId)
                    let state = !category.IsActiveNotDeleted ? InvalidCategoryState.Inactive
                        : !categoryBelongToFirm ? InvalidCategoryState.NotBelongToFirm
                            : InvalidCategoryState.NotSet
                    where state != InvalidCategoryState.NotSet
                    select new Order.InvalidCategory
                        {
                            OrderId = order.Id,
                            CategoryId = category.Id,
                            OrderPositionId = orderPosition.Id,
                            PositionId = opa.PositionId,
                            MayNotBelongToFirm = position.BindingObjectType == Facts::Position.BindingObjectTypeCategoryMultipleAsterix,
                            State = state,
                        };

                return result;
            }

            public FindSpecification<Order.InvalidCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidCategory>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderBargainSignedLaterThanOrderAccessor : DataChangesHandler<Order.BargainSignedLaterThanOrder>, IStorageBasedDataObjectAccessor<Order.BargainSignedLaterThanOrder>
        {
            private readonly IQuery _query;

            public OrderBargainSignedLaterThanOrderAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderShouldNotBeSignedBeforeBargain,
                    };

            public IQueryable<Order.BargainSignedLaterThanOrder> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.BargainId.HasValue)
                   from bargain in _query.For<Facts::Bargain>().Where(x => x.Id == order.BargainId)
                   where bargain.SignupDate > order.SignupDate
                   select new Order.BargainSignedLaterThanOrder
                   {
                       OrderId = order.Id,
                       BargainId = order.BargainId.Value,
                   };

            public FindSpecification<Order.BargainSignedLaterThanOrder> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.BargainSignedLaterThanOrder>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderHasNoAnyLegalPersonProfileAccessor : DataChangesHandler<Order.HasNoAnyLegalPersonProfile>, IStorageBasedDataObjectAccessor<Order.HasNoAnyLegalPersonProfile>
        {
            private readonly IQuery _query;

            public OrderHasNoAnyLegalPersonProfileAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile,
                    };

            public IQueryable<Order.HasNoAnyLegalPersonProfile> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.LegalPersonId == order.LegalPersonId).DefaultIfEmpty()
                   where profile == null
                   select new Order.HasNoAnyLegalPersonProfile
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.HasNoAnyLegalPersonProfile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.HasNoAnyLegalPersonProfile>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderHasNoAnyPositionAccessor : DataChangesHandler<Order.HasNoAnyPosition>, IStorageBasedDataObjectAccessor<Order.HasNoAnyPosition>
        {
            private readonly IQuery _query;

            public OrderHasNoAnyPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderShouldHaveAtLeastOnePosition,
                    };

            public IQueryable<Order.HasNoAnyPosition> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                   where orderPosition == null
                   select new Order.HasNoAnyPosition
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.HasNoAnyPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.HasNoAnyPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class InactiveReferenceAccessor : DataChangesHandler<Order.InactiveReference>, IStorageBasedDataObjectAccessor<Order.InactiveReference>
        {
            private readonly IQuery _query;

            public InactiveReferenceAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustHaveActiveDeal,
                        MessageTypeCode.OrderMustHaveActiveLegalEntities,
                    };

            // todo: сравнить запросы left join и exists
            public IQueryable<Order.InactiveReference> GetSource()
                => from order in _query.For<Facts::Order>()
                   from boou in _query.For<Facts::BranchOfficeOrganizationUnit>().Where(x => x.Id == order.BranchOfficeOrganizationUnitId).DefaultIfEmpty()
                   from bo in _query.For<Facts::BranchOffice>().Where(x => boou != null && x.Id == boou.BranchOfficeId).DefaultIfEmpty()
                   from legalPerson in _query.For<Facts::LegalPerson>().Where(x => x.Id == order.LegalPersonId).DefaultIfEmpty()
                   from legalPersonProfile in _query.For<Facts::LegalPersonProfile>().Where(x => x.Id == order.LegalPersonProfileId).DefaultIfEmpty()
                   from deal in _query.For<Facts::Deal>().Where(x => x.Id == order.DealId).DefaultIfEmpty()
                   where boou == null || bo == null || legalPerson == null || legalPersonProfile == null || deal == null
                   select new Order.InactiveReference
                       {
                           OrderId = order.Id,
                           BranchOfficeOrganizationUnit = order.BranchOfficeOrganizationUnitId != null && boou == null,
                           BranchOffice = boou != null && bo == null,
                           LegalPerson = order.LegalPersonId != null && legalPerson == null,
                           LegalPersonProfile = order.LegalPersonProfileId != null && legalPersonProfile == null,
                           Deal = order.DealId != null && deal == null,
                       };

            public FindSpecification<Order.InactiveReference> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InactiveReference>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidBeginDistributionDateAccessor : DataChangesHandler<Order.InvalidBeginDistributionDate>, IStorageBasedDataObjectAccessor<Order.InvalidBeginDistributionDate>
        {
            private readonly IQuery _query;

            public OrderInvalidBeginDistributionDateAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth,
                    };

            public IQueryable<Order.InvalidBeginDistributionDate> GetSource()
                => from order in _query.For<Facts::Order>()
                   where order.BeginDistribution.Day != 1 || order.BeginDistribution.TimeOfDay != TimeSpan.Zero
                   select new Order.InvalidBeginDistributionDate
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.InvalidBeginDistributionDate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidBeginDistributionDate>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidEndDistributionDateAccessor : DataChangesHandler<Order.InvalidEndDistributionDate>, IStorageBasedDataObjectAccessor<Order.InvalidEndDistributionDate>
        {
            private readonly IQuery _query;

            public OrderInvalidEndDistributionDateAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth,
                    };

            public IQueryable<Order.InvalidEndDistributionDate> GetSource()
                => from order in _query.For<Facts::Order>()
                   where order.EndDistributionPlan.Day != 1 || order.EndDistributionPlan.TimeOfDay != TimeSpan.Zero
                   select new Order.InvalidEndDistributionDate
                       {
                           OrderId = order.Id,
                       };

        public FindSpecification<Order.InvalidEndDistributionDate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidEndDistributionDate>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidBillsTotalAccessor : DataChangesHandler<Order.InvalidBillsTotal>, IStorageBasedDataObjectAccessor<Order.InvalidBillsTotal>
        {
            private readonly IQuery _query;

            public OrderInvalidBillsTotalAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.BillsSumShouldMatchOrder,
                    };

            public IQueryable<Order.InvalidBillsTotal> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.WorkflowStep == Facts::Order.State.OnRegistration && !x.IsFreeOfCharge)
                   let billTotal = _query.For<Facts::Bill>().Where(x => x.OrderId == order.Id).Sum(x => (decimal?)x.PayablePlan)
                   let orderTotal = (from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                     from rw in _query.For<Facts::ReleaseWithdrawal>().Where(x => x.OrderPositionId == op.Id)
                                     select rw.Amount).Sum()
                   where orderTotal > 0 && billTotal.HasValue && billTotal != orderTotal
                   select new Order.InvalidBillsTotal
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.InvalidBillsTotal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidBillsTotal>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class LegalPersonProfileBargainExpiredAccessor : DataChangesHandler<Order.LegalPersonProfileBargainExpired>, IStorageBasedDataObjectAccessor<Order.LegalPersonProfileBargainExpired>
        {
            private readonly IQuery _query;

            public LegalPersonProfileBargainExpiredAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired,
                    };

            public IQueryable<Order.LegalPersonProfileBargainExpired> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.BargainEndDate.HasValue).Where(x => x.LegalPersonId == order.LegalPersonId)
                   where profile.BargainEndDate.Value < order.SignupDate
                   select new Order.LegalPersonProfileBargainExpired
                   {
                       OrderId = order.Id,
                       LegalPersonProfileId = profile.Id,
                   };

            public FindSpecification<Order.LegalPersonProfileBargainExpired> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.LegalPersonProfileBargainExpired>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class LegalPersonProfileWarrantyExpiredAccessor : DataChangesHandler<Order.LegalPersonProfileWarrantyExpired>, IStorageBasedDataObjectAccessor<Order.LegalPersonProfileWarrantyExpired>
        {
            private readonly IQuery _query;

            public LegalPersonProfileWarrantyExpiredAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired,
                    };

            public IQueryable<Order.LegalPersonProfileWarrantyExpired> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.WarrantyEndDate.HasValue).Where(x => x.LegalPersonId == order.LegalPersonId)
                   where profile.WarrantyEndDate.Value < order.SignupDate
                   select new Order.LegalPersonProfileWarrantyExpired
                   {
                       OrderId = order.Id,
                       LegalPersonProfileId = profile.Id,
                   };

            public FindSpecification<Order.LegalPersonProfileWarrantyExpired> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.LegalPersonProfileWarrantyExpired>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingBargainScanAccessor : DataChangesHandler<Order.MissingBargainScan>, IStorageBasedDataObjectAccessor<Order.MissingBargainScan>
        {
            private readonly IQuery _query;

            public OrderMissingBargainScanAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.BargainScanShouldPresent,
                    };

            public IQueryable<Order.MissingBargainScan> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.BargainId.HasValue)
                   from scan in _query.For<Facts::BargainScanFile>().Where(x => x.BargainId == order.BargainId).DefaultIfEmpty()
                   where scan == null
                   select new Order.MissingBargainScan
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingBargainScan> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.MissingBargainScan>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingBillsAccessor : DataChangesHandler<Order.MissingBills>, IStorageBasedDataObjectAccessor<Order.MissingBills>
        {
            private readonly IQuery _query;

            public OrderMissingBillsAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.BillsShouldBeCreated,
                    };

            public IQueryable<Order.MissingBills> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.WorkflowStep == Facts::Order.State.OnRegistration && !x.IsFreeOfCharge)
                   let billCount = _query.For<Facts::Bill>().Count(x => x.OrderId == order.Id)
                   let orderTotal = (from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                     from rw in _query.For<Facts::ReleaseWithdrawal>().Where(x => x.OrderPositionId == op.Id)
                                     select rw.Amount).Sum()
                   where orderTotal > 0 && billCount == 0
                   select new Order.MissingBills
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingBills> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.MissingBills>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class MissingRequiredFieldAccessor : DataChangesHandler<Order.MissingRequiredField>, IStorageBasedDataObjectAccessor<Order.MissingRequiredField>
        {
            private readonly IQuery _query;

            public MissingRequiredFieldAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustHaveActiveDeal,
                        MessageTypeCode.OrderRequiredFieldsShouldBeSpecified,
                    };

            public IQueryable<Order.MissingRequiredField> GetSource()
                => from order in _query.For<Facts::Order>()
                   where !(order.BranchOfficeOrganizationUnitId.HasValue && order.CurrencyId.HasValue && order.LegalPersonId.HasValue && order.LegalPersonProfileId.HasValue && order.DealId.HasValue)
                   select new Order.MissingRequiredField
                       {
                           OrderId = order.Id,
                           BranchOfficeOrganizationUnit = !order.BranchOfficeOrganizationUnitId.HasValue,
                           Currency = !order.CurrencyId.HasValue,
                           LegalPerson = !order.LegalPersonId.HasValue,
                           LegalPersonProfile = !order.LegalPersonProfileId.HasValue,
                           Deal = !order.DealId.HasValue,
                       };

            public FindSpecification<Order.MissingRequiredField> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.MissingRequiredField>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingOrderScanAccessor : DataChangesHandler<Order.MissingOrderScan>, IStorageBasedDataObjectAccessor<Order.MissingOrderScan>
        {
            private readonly IQuery _query;

            public OrderMissingOrderScanAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderScanShouldPresent,
                    };

            public IQueryable<Order.MissingOrderScan> GetSource()
                => from order in _query.For<Facts::Order>()
                   from scan in _query.For<Facts::OrderScanFile>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                   where scan == null
                   select new Order.MissingOrderScan
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingOrderScan> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.MissingOrderScan>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}