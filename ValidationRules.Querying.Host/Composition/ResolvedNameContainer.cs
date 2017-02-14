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
                var packagePosition = Get<EntityTypePosition>(reference).First();
                return new NamedReference(EntityTypeOrderPosition.Instance, reference.Id, For(packagePosition).Name);
            }

            NamedReference name;
            return _names.TryGetValue(reference, out name) ? name : new NamedReference(null, 0, "Not resolved");
        }

        private static IEnumerable<Reference> Get<TEntityType>(Reference messageParams)
            where TEntityType : IdentityBase<TEntityType>, new()
            => messageParams.Children.Where(x => x.EntityType == EntityTypeBase<TEntityType>.Instance.Id);
    }
}