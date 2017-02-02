using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication
{
    public sealed class SyncEntityNameActor : IActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<EntityName> _bulkRepository;
        private readonly IEqualityComparer<EntityName> _identityComparer;
        private readonly IEqualityComparer<EntityName> _completeComparer;

        public SyncEntityNameActor(IQuery query,
                                   IBulkRepository<EntityName> bulkRepository,
                                   IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<EntityName>();
            _completeComparer = equalityComparerFactory.CreateCompleteComparer<EntityName>();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            foreach (var accessor in CreateAccessors(_query))
            {
                var specification = accessor.GetFindSpecification(commands);

                var dataChangesDetector = new TwoPhaseDataChangesDetector<EntityName>(
                    spec => accessor.GetSource().WhereMatched(spec),
                    spec => _query.For<EntityName>().WhereMatched(spec),
                    _identityComparer,
                    _completeComparer);

                var changes = dataChangesDetector.DetectChanges(specification);
                _bulkRepository.Delete(changes.Complement);
                _bulkRepository.Create(changes.Difference);
                _bulkRepository.Update(changes.Intersection);
            }

            return Array.Empty<IEvent>();
        }

        private static IEnumerable<IStorageBasedDataObjectAccessor<EntityName>> CreateAccessors(IQuery query)
        {
            return new IStorageBasedDataObjectAccessor<EntityName>[]
            {
                new AdvertisementNameAccessor(query),
                new AdvertisementElementTemplateNameAccessor(query),
                new CategoryNameAccessor(query),
                new FirmNameAccessor(query),
                new FirmAddressNameAccessor(query),
                new LegalPersonProfileNameAccessor(query),
                new OrderNameAccessor(query),
                new PositionNameAccessor(query),
                new ProjectNameAccessor(query),
                new ThemeNameAccessor(query),
            };
        }

        public sealed class AdvertisementNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public AdvertisementNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Advertisement)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Advertisement,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Advertisement),
                    EntityTypeIds.Advertisement);
        }

        public sealed class AdvertisementElementTemplateNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public AdvertisementElementTemplateNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.AdvertisementElementTemplate)
                .Select(x => new EntityName
                    {
                        Id = x.Id,
                        TypeId = EntityTypeIds.AdvertisementElementTemplate,
                        Name = x.Name,
                    });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(AdvertisementElementTemplate),
                    EntityTypeIds.AdvertisementElementTemplate);
        }

        public sealed class CategoryNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public CategoryNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Category)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Category,
                    Name = x.Name
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Category),
                    EntityTypeIds.Category);
        }

        public sealed class FirmNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public FirmNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Firm)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Firm,
                    Name = x.Name
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Firm),
                    EntityTypeIds.Firm);
        }

        public sealed class FirmAddressNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public FirmAddressNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.FirmAddress)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.FirmAddress,
                    Name = x.Address
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(FirmAddress),
                    EntityTypeIds.FirmAddress);
        }

        public sealed class LegalPersonProfileNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public LegalPersonProfileNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.LegalPersonProfile)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.LegalPersonProfile,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(LegalPersonProfile),
                    EntityTypeIds.LegalPersonProfile);
        }

        public sealed class OrderNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public OrderNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Order)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Order,
                    Name = x.Number
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Order),
                    EntityTypeIds.Order);
        }

        public sealed class PositionNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public PositionNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Position)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Position,
                    Name = x.Name
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Position),
                    EntityTypeIds.Position);
        }

        public sealed class ProjectNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public ProjectNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Project)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Project,
                    Name = x.DisplayName
                });

            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Project),
                    EntityTypeIds.Project);
        }

        public sealed class ThemeNameAccessor : IStorageBasedDataObjectAccessor<EntityName>
        {
            private readonly IQuery _query;

            public ThemeNameAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<EntityName> GetSource() => _query
                .For(Specs.Find.Erm.Theme)
                .Select(x => new EntityName
                {
                    Id = x.Id,
                    TypeId = EntityTypeIds.Theme,
                    Name = x.Name
                });


            public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => SyncEntityNameActor.GetFindSpecification(commands,
                    typeof(Theme),
                    EntityTypeIds.Theme);
        }

        private static FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands, Type type, int typeId)
        {
            var ids = commands.Cast<SyncDataObjectCommand>()
                              .Where(c => c.DataObjectType == type)
                              .Distinct()
                              .Select(c => new EntityNameKey
                              {
                                  Id = c.DataObjectId,
                                  TypeId = typeId
                              }).ToList();

            return SpecificationFactory<EntityName>.Contains(x => new EntityNameKey { Id = x.Id, TypeId = x.TypeId }, ids);
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private struct EntityNameKey
        {
            public long Id { get; set; }
            public int TypeId { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}