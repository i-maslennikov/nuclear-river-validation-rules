using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.ValidationRules.Domain.EntityTypes;
using NuClear.ValidationRules.Domain.Model.Facts;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    public sealed class FactDependencyTests
    {
        class AssociatedPositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<AssociatedPosition>
                    .Given(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 },
                           new PricePosition { Id = 3, PriceId = 4 })
                    .Create(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }

            [Test]
            public void Update()
            {
                CommandValidation<AssociatedPosition>
                    .Given(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2 },
                           new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 },
                           new PricePosition { Id = 3, PriceId = 4 },
                           new AssociatedPositionsGroup { Id = 5, PricePositionId = 6 },
                           new PricePosition { Id = 6, PriceId = 7 })
                    .Update(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 5 })
                    .Expect(Recalculate<EntityTypePrice>(4), Recalculate<EntityTypePrice>(7));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<AssociatedPosition>
                    .Given(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2 },
                           new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 },
                           new PricePosition { Id = 3, PriceId = 4 })
                    .Delete(new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 2 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }
        }

        class AssociatedPositionsGroupTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<AssociatedPositionsGroup>
                    .Given(new PricePosition { Id = 3, PriceId = 4 })
                    .Create(new AssociatedPositionsGroup { Id = 1, PricePositionId = 3 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }

            [Test]
            public void Update()
            {
                CommandValidation<AssociatedPositionsGroup>
                    .Given(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 },
                           new PricePosition { Id = 3, PriceId = 4 },
                           new PricePosition { Id = 6, PriceId = 7 })
                    .Update(new AssociatedPositionsGroup { Id = 2, PricePositionId = 6 })
                    .Expect(Recalculate<EntityTypePrice>(4), Recalculate<EntityTypePrice>(7));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<AssociatedPositionsGroup>
                    .Given(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 },
                           new PricePosition { Id = 3, PriceId = 4 })
                    .Delete(new AssociatedPositionsGroup { Id = 2, PricePositionId = 3 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }
        }

        class DeniedPositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<DeniedPosition>
                    .Given()
                    .Create(new DeniedPosition { Id = 1, PriceId = 4 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }

            [Test]
            public void Update()
            {
                CommandValidation<DeniedPosition>
                    .Given(new DeniedPosition { Id = 2, PriceId = 4 })
                    .Update(new DeniedPosition { Id = 2, PriceId = 7 })
                    .Expect(Recalculate<EntityTypePrice>(4), Recalculate<EntityTypePrice>(7));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<DeniedPosition>
                    .Given(new DeniedPosition { Id = 2, PriceId = 4 })
                    .Delete(new DeniedPosition { Id = 2, PriceId = 4 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }
        }

        class GlobalAssociatedPositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<GlobalAssociatedPosition>
                    .Given()
                    .Create(new GlobalAssociatedPosition { Id = 1, RulesetId = 1 })
                    .Expect(Recalculate<EntityTypeRuleset>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<GlobalAssociatedPosition>
                    .Given(new GlobalAssociatedPosition { Id = 1, RulesetId = 1 })
                    .Update(new GlobalAssociatedPosition { Id = 1, RulesetId = 2 })
                    .Expect(Recalculate<EntityTypeRuleset>(1), Recalculate<EntityTypeRuleset>(2));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<GlobalAssociatedPosition>
                    .Given(new GlobalAssociatedPosition { Id = 1, RulesetId = 1 })
                    .Delete(new GlobalAssociatedPosition { Id = 1, RulesetId = 1 })
                    .Expect(Recalculate<EntityTypeRuleset>(1));
            }
        }

        class GlobalDeniedPositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<GlobalDeniedPosition>
                    .Given()
                    .Create(new GlobalDeniedPosition { Id = 1, RulesetId = 1 })
                    .Expect(Recalculate<EntityTypeRuleset>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<GlobalDeniedPosition>
                    .Given(new GlobalDeniedPosition { Id = 1, RulesetId = 1 })
                    .Update(new GlobalDeniedPosition { Id = 1, RulesetId = 2 })
                    .Expect(Recalculate<EntityTypeRuleset>(1), Recalculate<EntityTypeRuleset>(2));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<GlobalDeniedPosition>
                    .Given(new GlobalDeniedPosition { Id = 1, RulesetId = 1 })
                    .Delete(new GlobalDeniedPosition { Id = 1, RulesetId = 1 })
                    .Expect(Recalculate<EntityTypeRuleset>(1));
            }
        }

        class CategoryTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<Category>
                    .Given(new Order { Id = 1 },
                           new OrderPosition { Id = 2, OrderId = 1 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 })
                    .Create(new Category { Id = 3 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<Category>
                    .Given(new Order { Id = 1 },
                           new OrderPosition { Id = 2, OrderId = 1 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 },
                           new Category { Id = 3 })
                    .Update(new Category { Id = 3 })
                    .Expect(Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(1));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<Category>
                    .Given(new Order { Id = 1 },
                           new OrderPosition { Id = 2, OrderId = 1 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, CategoryId = 3 },
                           new Category { Id = 3 })
                    .Delete(new Category { Id = 3 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }
        }

        class OrderTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<Order>
                    .Given()
                    .Create(new Order { Id = 1, OwnerId = 1 })
                    .Expect(Initialize<EntityTypeOrder>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<Order>
                    .Given(new Order { Id = 1, OwnerId = 1 })
                    .Update(new Order { Id = 1, OwnerId = 1 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<Order>
                    .Given(new Order { Id = 1, OwnerId = 1 })
                    .Delete(new Order { Id = 1, OwnerId = 1 })
                    .Expect(Destroy<EntityTypeOrder>(1));
            }
        }

        class OrderPositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<OrderPosition>
                    .Given()
                    .Create(new OrderPosition { Id = 1, OrderId = 1 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<OrderPosition>
                    .Given(new OrderPosition { Id = 1, OrderId = 1 })
                    .Update(new OrderPosition { Id = 1, OrderId = 2 })
                    .Expect(Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(2));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<OrderPosition>
                    .Given(new OrderPosition { Id = 1, OrderId = 1 })
                    .Delete(new OrderPosition { Id = 1, OrderId = 1 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }
        }

        class OrderPositionAdvertisementTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<OrderPositionAdvertisement>
                    .Given(new OrderPosition { Id = 2, OrderId = 1 })
                    .Create(new OrderPositionAdvertisement { Id = 3, OrderPositionId = 2 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<OrderPositionAdvertisement>
                    .Given(new OrderPosition { OrderId = 1, Id = 2 },
                           new OrderPosition { OrderId = 2, Id = 3 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4 })
                    .Update(new OrderPositionAdvertisement { OrderPositionId = 3, Id = 4 })
                    .Expect(Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(2));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<OrderPositionAdvertisement>
                    .Given(new OrderPosition { OrderId = 1, Id = 2 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4 })
                    .Delete(new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4 })
                    .Expect(Recalculate<EntityTypeOrder>(1));
            }
        }

        class PositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<Position>
                    .Given(new OrderPosition { OrderId = 1, Id = 2 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4 },

                           new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 },
                           new PricePosition { Id = 13, PositionId = 4 })
                    .Create(new Position { Id = 4 })
                    .Expect(Initialize<EntityTypePosition>(4), Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(11));
            }

            [Test]
            public void Update()
            {
                CommandValidation<Position>
                    .Given(new OrderPosition { OrderId = 1, Id = 2 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4},

                           new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 },
                           new PricePosition { Id = 13, PositionId = 4 },

                           new Position { Id = 4 })
                    .Update(new Position { Id = 4 })
                    .Expect(Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(11), Recalculate<EntityTypePosition>(4), Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(11));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<Position>
                    .Given(new OrderPosition { OrderId = 1, Id = 2 },
                           new OrderPositionAdvertisement { OrderPositionId = 2, Id = 4, PositionId = 4 },

                           new OrderPosition { OrderId = 11, Id = 12, PricePositionId = 13 },
                           new PricePosition { Id = 13, PositionId = 4 },

                           new Position { Id = 4 })
                    .Delete(new Position { Id = 4 })
                    .Expect(Recalculate<EntityTypeOrder>(1), Recalculate<EntityTypeOrder>(11), Destroy<EntityTypePosition>(4));
            }
        }

        class PriceTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<Price>
                    .Given()
                    .Create(new Price { Id = 4 })
                    .Expect(Initialize<EntityTypePrice>(4));
            }

            [Test]
            public void Update()
            {
                CommandValidation<Price>
                    .Given(new Price { Id = 4 })
                    .Update(new Price { Id = 4 })
                    .Expect(Recalculate<EntityTypePrice>(4));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<Price>
                    .Given(new Price { Id = 4 })
                    .Delete(new Price { Id = 4 })
                    .Expect(Destroy<EntityTypePrice>(4));
            }
        }

        class PricePositionTests
        {
            [Test]
            public void Create()
            {
                CommandValidation<PricePosition>
                    .Given()
                    .Create(new PricePosition { Id = 4, PriceId = 1 })
                    .Expect(Recalculate<EntityTypePrice>(1));
            }

            [Test]
            public void Update()
            {
                CommandValidation<PricePosition>
                    .Given(new PricePosition { Id = 4, PriceId = 1 })
                    .Update(new PricePosition { Id = 4, PriceId = 2 })
                    .Expect(Recalculate<EntityTypePrice>(1), Recalculate<EntityTypePrice>(2));
            }

            [Test]
            public void Delete()
            {
                CommandValidation<PricePosition>
                    .Given(new PricePosition { Id = 4, PriceId = 1 })
                    .Delete(new PricePosition { Id = 4, PriceId = 1 })
                    .Expect(Recalculate<EntityTypePrice>(1));
            }
        }

        private static InitializeAggregate Initialize<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new InitializeAggregate(new EntityReference(EntityTypeBase<TEntityType>.Instance, id));

        private static RecalculateAggregate Recalculate<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new RecalculateAggregate(new EntityReference(EntityTypeBase<TEntityType>.Instance, id));

        private static DestroyAggregate Destroy<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new DestroyAggregate(new EntityReference(EntityTypeBase<TEntityType>.Instance, id));
    }
}
