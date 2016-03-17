using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Context;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.ValidationRules.Domain.EntityTypes;
using NuClear.ValidationRules.Domain.Model.Facts;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    public sealed class FactDependencyTests
    {
        [Test]
        public void TestOrderUpdate()
        {
            CommandValidation<Order>
                .Given(new Order { Id = 1, OwnerId = 1 })
                .Update(new Order { Id = 1, OwnerId = 1 })
                .Expect(Recalculate<EntityTypeOrder>(1));
        }

        [Test]
        public void TestOrderCreate()
        {
            CommandValidation<Order>
                .Given()
                .Create(new Order { Id = 1, OwnerId = 1 })
                .Expect(Initialize<EntityTypeOrder>(1));
        }

        [Test]
        public void TestOrderDelete()
        {
            CommandValidation<Order>
                .Given(new Order { Id = 1, OwnerId = 1 })
                .Delete(new Order { Id = 1, OwnerId = 1 })
                .Expect(Destroy<EntityTypeOrder>(1));
        }

        private static InitializeAggregate Initialize<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new InitializeAggregate(PredicateFactory.EntityById(EntityTypeBase<TEntityType>.Instance, id));

        private static RecalculateAggregate Recalculate<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new RecalculateAggregate(PredicateFactory.EntityById(EntityTypeBase<TEntityType>.Instance, id));

        private static DestroyAggregate Destroy<TEntityType>(long id) where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
            => new DestroyAggregate(PredicateFactory.EntityById(EntityTypeBase<TEntityType>.Instance, id));
    }
}
