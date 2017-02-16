using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public sealed class ResolvedNameContainer
    {
        private readonly IReadOnlyDictionary<Reference, NamedReference> _names;

        public ResolvedNameContainer(IReadOnlyDictionary<Reference, string> names)
        {
            _names = names.ToDictionary(x => x.Key, x => new NamedReference(x.Key, x.Value), Reference.Comparer);
        }

        public NamedReference For(Reference reference)
        {
            if (reference.EntityType == EntityTypeOrderPosition.Instance.Id)
            {
                // Ссылки на op включают в себя заказ и одну или две номенклатурных позиций.
                var order = Get<EntityTypeOrder>(reference).First();
                var packagePosition = Get<EntityTypePosition>(reference).First();
                var itemPosition = Get<EntityTypePosition>(reference).Last();
                return new OrderPositionNamedReference(reference, For(packagePosition), For(itemPosition), For(order));
            }

            if (reference.EntityType == EntityTypeOrderPositionAdvertisement.Instance.Id)
            {
                // Сслыки на opa - всегда представляются в виде ссылок на op, только имя подставляется дочерней позиции.
                var orderPosition = Get<EntityTypeOrderPosition>(reference).First();
                var itemPosition = Get<EntityTypePosition>(reference).First();
                return new NamedReference(orderPosition, For(itemPosition).Name);
            }

            if (reference.EntityType == EntityTypeAdvertisementElement.Instance.Id)
            {
                var template = Get<EntityTypeAdvertisementElementTemplate>(reference).First();
                return new NamedReference(reference, For(template).Name);
            }

            if (reference.EntityType == EntityTypePricePosition.Instance.Id)
            {
                var position = Get<EntityTypePosition>(reference).First();
                return new NamedReference(reference, For(position).Name);
            }

            NamedReference name;
            return _names.TryGetValue(reference, out name) ? name : new NamedReference(reference, "Not resolved");
        }

        private static IEnumerable<Reference> Get<TEntityType>(Reference messageParams)
            where TEntityType : IdentityBase<TEntityType>, new()
            => messageParams.Children.Where(x => x.EntityType == EntityTypeBase<TEntityType>.Instance.Id);
    }
}