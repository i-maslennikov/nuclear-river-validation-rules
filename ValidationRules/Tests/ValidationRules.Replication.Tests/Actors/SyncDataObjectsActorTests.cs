using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.ValidationRules.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests : ActorFixtureBase
    {
        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionCreated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2, IsActive = true })
                    .Has(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 })
                    .Has(new PricePosition { Id = 3, PriceId = 4 });

            Actor.Create(Store)
                 .Sync<AssociatedPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionUpdated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2, IsActive = true })
                    .Has(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 5 })

                    .Has(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 })
                    .Has(new PricePosition { Id = 3, PriceId = 4 })
                    .Has(new AssociatedPositionsGroup { Id = 5, PricePositionId = 6 })
                    .Has(new PricePosition { Id = 6, PriceId = 7 });

            Actor.Create(Store)
                 .Sync<AssociatedPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4), DataObject.RelatedDataObjectOutdated<Price>(7));
        }

        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionDeleted()
        {
            Store.Builder
                    .Has(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2 })
                    .Has(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 })
                    .Has(new PricePosition { Id = 3, PriceId = 4 });

            Actor.Create(Store)
                 .Sync<AssociatedPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionGroupCreated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.AssociatedPositionsGroup { Id = 1, PricePositionId = 3, IsActive = true })
                    .Has(new PricePosition { Id = 3, PriceId = 4 });

            Actor.Create(Store)
                 .Sync<AssociatedPositionsGroup>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionGroupUpdated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.AssociatedPositionsGroup { Id = 2, PricePositionId = 3, IsActive = true })
                    .Has(new AssociatedPositionsGroup { Id = 2, PricePositionId = 6 })
                    .Has(new PricePosition { Id = 3, PriceId = 4 })
                    .Has(new PricePosition { Id = 6, PriceId = 7 });

            Actor.Create(Store)
                 .Sync<AssociatedPositionsGroup>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4), DataObject.RelatedDataObjectOutdated<Price>(7));
        }

        [Test]
        public void ShouldRecalculatePriceIfAssociatedPositionGroupDeleted()
        {
            Store.Builder
                    .Has(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 })
                    .Has(new PricePosition { Id = 3, PriceId = 4 })
                    .Has(new PricePosition { Id = 6, PriceId = 7 });

            Actor.Create(Store)
                 .Sync<AssociatedPositionsGroup>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfDeniedPositionCreated()
        {
            Store.Builder
                 .Has(new Storage.Model.Erm.DeniedPosition { Id = 1, PriceId = 4, IsActive = true });

            Actor.Create(Store)
                 .Sync<DeniedPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfDeniedPositionUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.DeniedPosition { Id = 2, PriceId = 4, IsActive = true })
                .Has(new DeniedPosition { Id = 2, PriceId = 7 });

            Actor.Create(Store)
                 .Sync<DeniedPosition>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4), DataObject.RelatedDataObjectOutdated<Price>(7));
        }

        [Test]
        public void ShouldRecalculatePriceIfDeniedPositionDeleted()
        {
            Store.Builder
                .Has(new DeniedPosition { Id = 2, PriceId = 4 });

            Actor.Create(Store)
                 .Sync<DeniedPosition>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(4));
        }

        [Test]
        public void ShouldRecalculateRulesetIfRulesetRuleCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.RulesetRule { RulesetId = 1 })
                .Has(new Storage.Model.Erm.Ruleset { Id = 1, Priority = 1 });

            Actor.Create(Store)
                 .Sync<RulesetRule>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<RulesetRule>(1));
        }

        [Test]
        public void ShouldRecalculateRulesetIfRulesetRuleUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.RulesetRule { RulesetId = 1 })
                .Has(new Storage.Model.Erm.Ruleset { Id = 1, Priority = 1 })
                .Has(new RulesetRule { Id = 2 });

            Actor.Create(Store)
                 .Sync<RulesetRule>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<RulesetRule>(1), DataObject.RelatedDataObjectOutdated<RulesetRule>(2));
        }

        [Test]
        public void ShouldRecalculateRulesetIfRulesetRuleDeleted()
        {
            Store.Builder
                .Has(new RulesetRule { Id = 1 });

            Actor.Create(Store)
                 .Sync<RulesetRule>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<RulesetRule>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfCategoryCreated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.Category { Id = 3, IsActive = true })
                    .Has(new OrderPosition { Id = 2, OrderId = 1 })
                    .Has(new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 });

            Actor.Create(Store)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfCategoryUpdated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.Category { Id = 3, ParentId = 1, IsActive = true })
                    .Has(new Category { Id = 3, ParentId = 2 })
                    .Has(new OrderPosition { Id = 2, OrderId = 1 })
                    .Has(new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 });

            Actor.Create(Store)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfCategoryDeleted()
        {
            Store.Builder
                    .Has(new Category { Id = 3 })
                    .Has(new OrderPosition { Id = 2, OrderId = 1 })
                    .Has(new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 });

            Actor.Create(Store)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }

        [Test]
        public void ShouldInitializeOrderIfOrderCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.Order { Id = 1, DestOrganizationUnitId = 1, SourceOrganizationUnitId = 1, IsActive = true })
                .Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 1, IsActive = true });

            Actor.Create(Store)
                 .Sync<Order>(1)
                 .VerifyDistinct(e => e is DataObjectCreatedEvent, DataObject.Created<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.Order { Id = 1, DestOrganizationUnitId = 1, SourceOrganizationUnitId = 1, IsActive = true })
                .Has(new Order { Id = 1 })
                .Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 1, IsActive = true });

            Actor.Create(Store)
                 .Sync<Order>(1)
                 .VerifyDistinct(e => e is DataObjectUpdatedEvent, DataObject.Updated<Order>(1));
        }

        [Test]
        public void ShouldDestroyOrderIfOrderDeleted()
        {
            Store.Builder
                .Has(new Order { Id = 1 });

            Actor.Create(Store)
                 .Sync<Order>(1)
                 .VerifyDistinct(e => e is DataObjectDeletedEvent, DataObject.Deleted<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.OrderPosition { Id = 1, OrderId = 1, IsActive = true });

            Actor.Create(Store)
                 .Sync<OrderPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.OrderPosition { Id = 1, OrderId = 1, IsActive = true })
                .Has(new OrderPosition { Id = 1, OrderId = 2 });

            Actor.Create(Store)
                 .Sync<OrderPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1), DataObject.RelatedDataObjectOutdated<Order>(2));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionDeleted()
        {
            Store.Builder
                .Has(new OrderPosition { Id = 1, OrderId = 2 });

            Actor.Create(Store)
                 .Sync<OrderPosition>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(2));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionAdvertisementCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.OrderPositionAdvertisement { Id = 3, OrderPositionId = 2 })
                .Has(new OrderPosition { Id = 2, OrderId = 1 });

            Actor.Create(Store)
                 .Sync<OrderPositionAdvertisement>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionAdvertisementUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.OrderPositionAdvertisement { OrderPositionId = 2, Id = 4 })
                .Has(new OrderPositionAdvertisement { OrderPositionId = 3, Id = 4 })
                .Has(new OrderPosition { OrderId = 1, Id = 2 })
                .Has(new OrderPosition { OrderId = 2, Id = 3 });


            Actor.Create(Store)
                 .Sync<OrderPositionAdvertisement>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1), DataObject.RelatedDataObjectOutdated<Order>(2));
        }

        [Test]
        public void ShouldRecalculateOrderIfOrderPositionAdvertisementDeleted()
        {
            Store.Builder
                .Has(new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4 })
                .Has(new OrderPosition { OrderId = 1, Id = 2 });

            Actor.Create(Store)
                 .Sync<OrderPositionAdvertisement>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Order>(1));
        }


        [Test]
        public void ShouldInitializePositionIfPositionCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.Position { Id = 4, IsDeleted = false })
                .Has(new OrderPosition { OrderId = 1, Id = 2 })
                .Has(new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4 })
                .Has(new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 })
                .Has(new PricePosition { Id = 13, PositionId = 4 });

            Actor.Create(Store)
                 .Sync<Position>(4)
                 .VerifyDistinct(DataObject.Created<Position>(4), DataObject.RelatedDataObjectOutdated<Order>(1), DataObject.RelatedDataObjectOutdated<Order>(11));
        }

        [Test]
        public void ShouldRecalculateOrderIfPositionUpdated()
        {
            Store.Builder
                    .Has(new Storage.Model.Erm.Position { Id = 4, Name = "1", IsDeleted = false })
                    .Has(new Position { Id = 4, Name = "2" })
                    .Has(new OrderPosition { OrderId = 1, Id = 2 })
                    .Has(new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4 })
                    .Has(new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 })
                    .Has(new PricePosition { Id = 13, PositionId = 4 });

            Actor.Create(Store)
                 .Sync<Position>(4)
                 .VerifyDistinct(DataObject.Updated<Position>(4), DataObject.RelatedDataObjectOutdated<Order>(1), DataObject.RelatedDataObjectOutdated<Order>(11));
        }

        [Test]
        public void ShouldDestroyPositionIfPositionDeleted()
        {
            Store.Builder
                .Has(new Position { Id = 4 })
                .Has(new OrderPosition { OrderId = 1, Id = 2 })
                .Has(new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4 })
                .Has(new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 })
                .Has(new PricePosition { Id = 13, PositionId = 4 });

            Actor.Create(Store)
                 .Sync<Position>(4)
                 .VerifyDistinct(DataObject.Deleted<Position>(4), DataObject.RelatedDataObjectOutdated<Order>(1), DataObject.RelatedDataObjectOutdated<Order>(11));
        }


        [Test]
        public void ShouldInitializePriceIfPriceCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.Price { Id = 4, OrganizationUnitId = 1, IsActive = true, IsPublished = true })
                .Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 1, IsActive = true });

            Actor.Create(Store)
                 .Sync<Price>(4)
                 .VerifyDistinct(e => e is DataObjectCreatedEvent, DataObject.Created<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfPriceUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.Price { Id = 4, OrganizationUnitId = 1, IsActive = true, IsPublished = true })
                .Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 1, IsActive = true })
                .Has(new Price { Id = 4, OrganizationUnitId = 2 });

            Actor.Create(Store)
                 .Sync<Price>(4)
                 .VerifyDistinct(e => e is DataObjectUpdatedEvent, DataObject.Updated<Price>(4));
        }

        [Test]
        public void ShouldDestroyPriceIfPriceDeleted()
        {
            Store.Builder
                .Has(new Price { Id = 4 });

            Actor.Create(Store)
                 .Sync<Price>(4)
                 .VerifyDistinct(e => e is DataObjectDeletedEvent, DataObject.Deleted<Price>(4));
        }

        [Test]
        public void ShouldRecalculatePriceIfPricePositionCreated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.PricePosition { Id = 4, PriceId = 1, IsActive = true });

            Actor.Create(Store)
                 .Sync<PricePosition>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(1));
        }

        [Test]
        public void ShouldRecalculatePriceIfPricePositionUpdated()
        {
            Store.Builder
                .Has(new Storage.Model.Erm.PricePosition { Id = 4, PriceId = 1, IsActive = true })
                .Has(new PricePosition { Id = 4, PriceId = 2 });

            Actor.Create(Store)
                 .Sync<PricePosition>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(1), DataObject.RelatedDataObjectOutdated<Price>(2));
        }

        [Test]
        public void ShouldRecalculatePriceIfPricePositionDeleted()
        {
            Store.Builder
                .Has(new PricePosition { Id = 4, PriceId = 1 });

            Actor.Create(Store)
                 .Sync<PricePosition>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Price>(1));
        }

        private class Actor
        {
            private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes = GetImplementations(typeof(IStorageBasedDataObjectAccessor<>));
            private static readonly IReadOnlyDictionary<Type, Type> DataChangesHandlerTypes = GetImplementations(typeof(IDataChangesHandler<>));

            private readonly Store _store;
            private readonly List<IEvent> _events;

            private Actor(Store store)
            {
                _store = store;
                _events = new List<IEvent>();
            }

            public static Actor Create(Store store)
            {
                return new Actor(store);
            }

            public Actor Sync<TDataObject>(params long[] ids) where TDataObject : class
            {
                var commands = ids.Select(x => new SyncDataObjectCommand(typeof(TDataObject), x)).ToArray();
                return Sync<TDataObject>(commands);
            }

            public Actor Sync<TDataObject>(params SyncDataObjectCommand[] commands) where TDataObject : class
            {
                var accessor = CreateAccessor<TDataObject>(_store.Query);
                var dataChangesHandler = CreateDataChangesHandler<TDataObject>(_store.Query);
                var actor = new SyncDataObjectsActor<TDataObject>(_store.Query, _store.RepositoryFactory.Create<TDataObject>(), _store.EqualityComparerFactory, accessor, dataChangesHandler);

                _events.AddRange(actor.ExecuteCommands(commands));

                return this;
            }

            public void VerifyDistinct(params IEvent[] events)
            {
                Assert.That(_events.Distinct(), Is.EquivalentTo(events));
            }

            public void VerifyDistinct(Func<IEvent, bool> filter, params IEvent[] operations)
            {
                Assert.That(_events.Distinct().Where(filter), Is.EquivalentTo(operations));
            }

            private static IStorageBasedDataObjectAccessor<TDataObject> CreateAccessor<TDataObject>(IQuery query)
            {
                var accessorType = AccessorTypes[typeof(TDataObject)];
                var accessorInstance = Activator.CreateInstance(accessorType, query);
                return (IStorageBasedDataObjectAccessor<TDataObject>)accessorInstance;
            }

            private static IDataChangesHandler<TDataObject> CreateDataChangesHandler<TDataObject>(IQuery query)
            {
                var dataChangesHandlerType = DataChangesHandlerTypes[typeof(TDataObject)];
                var dataChangesHandlerInstance = Activator.CreateInstance(dataChangesHandlerType, query);
                return (IDataChangesHandler<TDataObject>)dataChangesHandlerInstance;
            }

            private static IReadOnlyDictionary<Type, Type> GetImplementations(Type interfaceType)
            {
                return (from type in typeof(Specs).Assembly.ExportedTypes
                        from @interface in type.GetInterfaces()
                        where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType
                        select new { GenericArgument = @interface.GetGenericArguments()[0], Type = type })
                    .ToDictionary(x => x.GenericArgument, x => x.Type);
            }
        }
    }
}