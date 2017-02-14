using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public sealed class ResolvedNameContainer
    {
        private readonly IReadOnlyDictionary<Reference, NamedReference> _names;

        public ResolvedNameContainer(IReadOnlyDictionary<Reference, string> names, IReadOnlyDictionary<int, IEntityType> knownEntityTypes)
        {
            _names = names.ToDictionary(x => x.Key, x => new NamedReference(knownEntityTypes[x.Key.EntityType], x.Key.Id, x.Value));
        }

        public NamedReference For(Reference reference)
        {
            if (reference.EntityType == EntityTypeOrderPosition.Instance.Id)
            {
                var order = Get<EntityTypeOrder>(reference).First();
                var packagePosition = Get<EntityTypePosition>(reference).First();
                var itemPosition = Get<EntityTypePosition>(reference).Last();
                return new OrderPositionNamedReference(reference.Id, For(packagePosition), For(itemPosition), For(order));
            }

            NamedReference name;
            return _names.TryGetValue(reference, out name) ? name : new NamedReference(null, 0, "Not resolved");
        }

        private static IEnumerable<Reference> Get<TEntityType>(Reference messageParams)
            where TEntityType : IdentityBase<TEntityType>, new()
            => messageParams.Children.Where(x => x.EntityType == EntityTypeBase<TEntityType>.Instance.Id);
    }
}