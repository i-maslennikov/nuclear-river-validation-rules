using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.River.Common.Metadata.Builders
{
    public sealed class StructuralModelElementBuilder : MetadataElementBuilder<StructuralModelElementBuilder, StructuralModelElement>
    {
        private string _name;
        private IEnumerable<IStructuralModelTypeElement> _types = Enumerable.Empty<IStructuralModelTypeElement>();
        private IEnumerable<EntityElementBuilder> _elements = Enumerable.Empty<EntityElementBuilder>();

        public StructuralModelElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public StructuralModelElementBuilder Types<TElement>(params TElement[] types)
            where TElement : IStructuralModelTypeElement
        {
            _types = _types.Concat(types.Cast<IStructuralModelTypeElement>());
            return this;
        }

        public StructuralModelElementBuilder Elements(params EntityElementBuilder[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            _elements = _elements.Concat(elements);
            return this;
        }

        protected override StructuralModelElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The model name was not specified.");
            }

            IEnumerable<EntityElement> entities;
            IEnumerable<IMetadataElement> types;
            ProcessElements(out entities, out types);

            Childs(types.ToArray());

            return new StructuralModelElement(_name.AsUri().AsIdentity(), entities, Features);
        }

        private void ProcessElements(out IEnumerable<EntityElement> roots, out IEnumerable<IMetadataElement> types)
        {
            var typesById = _types.ToDictionary(x => x.Identity.Id, x => (IMetadataElement) x);
            var rootEntities = new List<EntityElement>();

            foreach (var rootElement in _elements)
            {
                Visit(rootElement,
                      entityConfig =>
                      {
                          foreach (var propertyConfig in entityConfig.PropertyConfigs)
                          {
                              propertyConfig.OfType(LookupOrCreateElement(typesById, propertyConfig.TypeReference, () => propertyConfig.TypeElement));
                          }

                          foreach (var relationConfig in entityConfig.RelationConfigs)
                          {
                              relationConfig.DirectTo(LookupOrCreateElement(typesById, relationConfig.TargetEntityReference, () => (EntityElement)relationConfig.TargetEntityElementConfig));
                          }

                          LookupOrCreateElement(typesById, entityConfig.EntityId, () => (EntityElement)entityConfig);
                      });

                var rootId = rootElement.EntityId;
                rootEntities.Add(LookupOrCreateElement(typesById, rootId, () => (EntityElement)rootElement));
            }

            roots = rootEntities;
            types = typesById.Values;
        }

        private static void Visit(EntityElementBuilder entityConfig, Action<EntityElementBuilder> visitor)
        {
            foreach (var relation in entityConfig.RelationConfigs.Where(x => x.TargetEntityElementConfig != null))
            {
                Visit(relation.TargetEntityElementConfig, visitor);
            }
            visitor(entityConfig);
        }

        private static TElement LookupOrCreateElement<TElement>(IDictionary<Uri, IMetadataElement> dictionary, Uri entityId, Func<TElement> builder)
            where TElement : IMetadataElement
        {
            IMetadataElement element;
            if (!dictionary.TryGetValue(entityId, out element))
            {
                dictionary.Add(entityId, element = builder());
            }
            return (TElement)element;
        }
    }
}