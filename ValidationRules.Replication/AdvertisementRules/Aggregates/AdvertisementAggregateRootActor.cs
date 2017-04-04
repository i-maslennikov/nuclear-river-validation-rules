using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class AdvertisementAggregateRootActor : AggregateRootActor<Advertisement>
    {
        public AdvertisementAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Advertisement> bulkRepository,
            IBulkRepository<Advertisement.AdvertisementWebsite> advertisementWebsiteBulkRepository,
            IBulkRepository<Advertisement.RequiredElementMissing> requiredElementMissingBulkRepository,
            IBulkRepository<Advertisement.ElementNotPassedReview> elementInvalidBulkRepository,
            IBulkRepository<Advertisement.Coupon> couponRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new AdvertisementAccessor(query), bulkRepository,
                HasValueObject(new AdvertisementWebsiteAccessor(query), advertisementWebsiteBulkRepository),
                HasValueObject(new RequiredElementMissingAccessor(query), requiredElementMissingBulkRepository),
                HasValueObject(new CouponAccessor(query), couponRepository),
                HasValueObject(new ElementNotPassedReviewAccessor(query), elementInvalidBulkRepository));
        }

        public sealed class AdvertisementAccessor : DataChangesHandler<Advertisement>, IStorageBasedDataObjectAccessor<Advertisement>
        {
            private readonly IQuery _query;

            public AdvertisementAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementElementMustPassReview,
                        MessageTypeCode.AdvertisementMustBelongToFirm,
                        MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite,
                        MessageTypeCode.CouponMustBeSoldOnceAtTime,
                        MessageTypeCode.OrderCouponPeriodMustNotBeLessFiveDays,
                        MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        MessageTypeCode.OrderMustHaveAdvertisement,
                        MessageTypeCode.WhiteListAdvertisementMayPresent,
                    };

            public IQueryable<Advertisement> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted && x.FirmId != null)
                   select new Advertisement
                   {
                       Id = advertisement.Id,
                       FirmId = advertisement.FirmId.Value,
                       IsSelectedToWhiteList = advertisement.IsSelectedToWhiteList,
                   };

            public FindSpecification<Advertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AdvertisementWebsiteAccessor : DataChangesHandler<Advertisement.AdvertisementWebsite>, IStorageBasedDataObjectAccessor<Advertisement.AdvertisementWebsite>
        {
            private readonly IQuery _query;

            public AdvertisementWebsiteAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite,
                    };

            public IQueryable<Advertisement.AdvertisementWebsite> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   from element in _query.For<Facts::AdvertisementElement>().Where(x => x.AdvertisementId == advertisement.Id)
                   from elementTemplate in _query.For<Facts::AdvertisementElementTemplate>().Where(x => x.Id == element.AdvertisementElementTemplateId)
                   where elementTemplate.IsAdvertisementLink // РМ - рекламная ссылка
                   where element.Text != null // нет смысла хранить null ссылки
                   select new Advertisement.AdvertisementWebsite
                   {
                       AdvertisementId = advertisement.Id,
                       Website = element.Text,
                   };

            public FindSpecification<Advertisement.AdvertisementWebsite> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Advertisement.AdvertisementWebsite>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class RequiredElementMissingAccessor : DataChangesHandler<Advertisement.RequiredElementMissing>, IStorageBasedDataObjectAccessor<Advertisement.RequiredElementMissing>
        {
            private readonly IQuery _query;

            public RequiredElementMissingAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustHaveAdvertisement,
                    };

            public IQueryable<Advertisement.RequiredElementMissing> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   join element in _query.For<Facts::AdvertisementElement>().Where(x => x.IsEmpty) on advertisement.Id equals element.AdvertisementId
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>().Where(x => x.IsRequired) on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   select new Advertisement.RequiredElementMissing
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = element.AdvertisementElementTemplateId
                   };

            public FindSpecification<Advertisement.RequiredElementMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Advertisement.RequiredElementMissing>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class ElementNotPassedReviewAccessor : DataChangesHandler<Advertisement.ElementNotPassedReview>, IStorageBasedDataObjectAccessor<Advertisement.ElementNotPassedReview>
        {
            private const int StatusInvalid = 2;
            private const int StatusDraft = 3;

            private readonly IQuery _query;

            public ElementNotPassedReviewAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementElementMustPassReview,
                    };

            public IQueryable<Advertisement.ElementNotPassedReview> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   join template in _query.For<Facts::AdvertisementTemplate>() on advertisement.AdvertisementTemplateId equals template.Id
                   where advertisement.Id != template.DummyAdvertisementId // РМ - не заглушка
                   join element in _query.For<Facts::AdvertisementElement>().Where(x => x.Status == StatusInvalid || x.Status == StatusDraft) on advertisement.Id equals element.AdvertisementId
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>().Where(x => x.NeedsValidation) on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   select new Advertisement.ElementNotPassedReview
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = element.AdvertisementElementTemplateId,

                       Status = element.Status == StatusInvalid ? Advertisement.ReviewStatus.Invalid :
                                                    element.Status == StatusDraft ? Advertisement.ReviewStatus.Draft :
                                                    Advertisement.ReviewStatus.NotSet,
                   };

            public FindSpecification<Advertisement.ElementNotPassedReview> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Advertisement.ElementNotPassedReview>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class CouponAccessor : DataChangesHandler<Advertisement.Coupon>, IStorageBasedDataObjectAccessor<Advertisement.Coupon>
        {
            public static DateTime BeginMonth(DateTime x) => new DateTime(x.AddDays(4).Year, x.AddDays(4).Month, 1);
            public static DateTime EndMonth(DateTime x) => new DateTime(x.AddDays(-4).Year, x.AddDays(-4).Month, 1).AddMonths(1);

            private readonly IQuery _query;

            public CouponAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderCouponPeriodMustNotBeLessFiveDays,
                        MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                    };

            public IQueryable<Advertisement.Coupon> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   from element in _query.For<Facts::AdvertisementElement>().Where(x => x.BeginDate.HasValue && x.EndDate.HasValue).Where(x => x.AdvertisementId == advertisement.Id)
                   let beginDate = element.BeginDate.Value
                   let endDate = element.EndDate.Value
                   select new Advertisement.Coupon
                       {
                           AdvertisementId = advertisement.Id,
                           AdvertisementElementId = element.Id,
                           DaysTotal = (int)(endDate - beginDate).TotalDays + 1,
                           BeginMonth = BeginMonth(beginDate),
                           EndMonth = EndMonth(endDate),
                       };

            public FindSpecification<Advertisement.Coupon> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Advertisement.Coupon>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }
    }
}
