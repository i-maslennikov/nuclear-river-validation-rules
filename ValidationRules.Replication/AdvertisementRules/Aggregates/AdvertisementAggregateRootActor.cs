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
            IBulkRepository<Advertisement.ElementOffsetInDays> elementPeriodOffsetBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new AdvertisementAccessor(query), bulkRepository,
                HasValueObject(new AdvertisementWebsiteAccessor(query), advertisementWebsiteBulkRepository),
                HasValueObject(new RequiredElementMissingAccessor(query), requiredElementMissingBulkRepository),
                HasValueObject(new ElementOffsetInDaysAccessor(query), elementPeriodOffsetBulkRepository),
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
                        MessageTypeCode.OrderMustHaveAdvertisement,
                        MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                        MessageTypeCode.WhiteListAdvertisementMayPresent,
                    };

            public IQueryable<Advertisement> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted && x.FirmId != null)
                   select new Advertisement
                   {
                       Id = advertisement.Id,
                       Name = advertisement.Name,
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
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
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
                   join element in _query.For<Facts::AdvertisementElement>() on advertisement.Id equals element.AdvertisementId
                   where element.IsEmpty // ЭРМ пустой
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>() on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   where elementTemplate.IsRequired // шаблон ЭРМ обязателен
                   select new Advertisement.RequiredElementMissing
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = elementTemplate.Id,
                   };

            public FindSpecification<Advertisement.RequiredElementMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
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
                   join element in _query.For<Facts::AdvertisementElement>() on advertisement.Id equals element.AdvertisementId
                   where element.Status == StatusInvalid || // ЭРМ выверен с ошибками
                         element.Status == StatusDraft      // ЭРМ - черновик
                   join elementTemplate in _query.For<Facts::AdvertisementElementTemplate>() on element.AdvertisementElementTemplateId equals elementTemplate.Id
                   where elementTemplate.NeedsValidation // ЭРМ должен быть выверен
                   select new Advertisement.ElementNotPassedReview
                   {
                       AdvertisementId = advertisement.Id,

                       AdvertisementElementId = element.Id,
                       AdvertisementElementTemplateId = elementTemplate.Id,

                       Status = element.Status == StatusInvalid ? Advertisement.ReviewStatus.Invalid :
                                                    element.Status == StatusDraft ? Advertisement.ReviewStatus.Draft :
                                                    Advertisement.ReviewStatus.NotSet,
                   };

            public FindSpecification<Advertisement.ElementNotPassedReview> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement.ElementNotPassedReview>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }

        public sealed class ElementOffsetInDaysAccessor : DataChangesHandler<Advertisement.ElementOffsetInDays>, IStorageBasedDataObjectAccessor<Advertisement.ElementOffsetInDays>
        {
            private readonly IQuery _query;

            public ElementOffsetInDaysAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                    };

            public IQueryable<Advertisement.ElementOffsetInDays> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   from element in _query.For<Facts::AdvertisementElement>().Where(x => x.AdvertisementId == advertisement.Id)
                   where element.BeginDate != null && element.EndDate != null
                   select new Advertisement.ElementOffsetInDays
                       {
                           AdvertisementId = advertisement.Id,
                           AdvertisementElementId = element.Id,
                           EndToBeginOffset = (int)(element.EndDate.Value - element.BeginDate.Value).TotalDays + 1,
                           EndToMonthBeginOffset = element.EndDate.Value.Day,
                           MonthEndToBeginOffset = DateTime.DaysInMonth(element.BeginDate.Value.Year, element.BeginDate.Value.Month) - element.BeginDate.Value.Day + 1
                       };

            public FindSpecification<Advertisement.ElementOffsetInDays> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Advertisement.ElementOffsetInDays>(x => aggregateIds.Contains(x.AdvertisementId));
            }
        }
    }
}
