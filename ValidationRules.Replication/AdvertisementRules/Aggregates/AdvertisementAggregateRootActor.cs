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

using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class AdvertisementAggregateRootActor : EntityActorBase<Advertisement>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Advertisement.AdvertisementWebsite> _advertisementWebsiteBulkRepository;
        private readonly IBulkRepository<Advertisement.RequiredElementMissing> _requiredElementMissingBulkRepository;
        private readonly IBulkRepository<Advertisement.ElementNotPassedReview> _elementInvalidBulkRepository;

        public AdvertisementAggregateRootActor(
            IQuery query,
            IBulkRepository<Advertisement> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Advertisement.AdvertisementWebsite> advertisementWebsiteBulkRepository,
            IBulkRepository<Advertisement.RequiredElementMissing> requiredElementMissingBulkRepository,
            IBulkRepository<Advertisement.ElementNotPassedReview> elementInvalidBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new AdvertisementAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _advertisementWebsiteBulkRepository = advertisementWebsiteBulkRepository;
            _requiredElementMissingBulkRepository = requiredElementMissingBulkRepository;
            _elementInvalidBulkRepository = elementInvalidBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Advertisement.AdvertisementWebsite>(_query, _advertisementWebsiteBulkRepository, _equalityComparerFactory, new AdvertisementWebsiteAccessor(_query)),
                    new ValueObjectActor<Advertisement.RequiredElementMissing>(_query, _requiredElementMissingBulkRepository, _equalityComparerFactory, new RequiredElementMissingAccessor(_query)),
                    new ValueObjectActor<Advertisement.ElementNotPassedReview>(_query, _elementInvalidBulkRepository, _equalityComparerFactory, new ElementNotPassedReviewAccessor(_query)),
                };

        public sealed class AdvertisementAccessor : IStorageBasedDataObjectAccessor<Advertisement>
        {
            private readonly IQuery _query;

            public AdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

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

        public sealed class AdvertisementWebsiteAccessor : IStorageBasedDataObjectAccessor<Advertisement.AdvertisementWebsite>
        {
            private readonly IQuery _query;

            public AdvertisementWebsiteAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Advertisement.AdvertisementWebsite> GetSource()
                => from advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted)
                   from element in _query.For<Facts::AdvertisementElement>().Where(x => x.AdvertisementId == advertisement.Id)
                   from elementTemplate in _query.For<Facts::AdvertisementElementTemplate>().Where(x => x.Id == element.AdvertisementElementTemplateId)
                   where elementTemplate.IsAdvertisementLink // РМ - рекламная ссылка
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

        public sealed class RequiredElementMissingAccessor : IStorageBasedDataObjectAccessor<Advertisement.RequiredElementMissing>
        {
            private readonly IQuery _query;

            public RequiredElementMissingAccessor(IQuery query)
            {
                _query = query;
            }

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

        public sealed class ElementNotPassedReviewAccessor : IStorageBasedDataObjectAccessor<Advertisement.ElementNotPassedReview>
        {
            private const int StatusInvalid = 2;
            private const int StatusDraft = 3;

            private readonly IQuery _query;

            public ElementNotPassedReviewAccessor(IQuery query)
            {
                _query = query;
            }

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
    }
}
