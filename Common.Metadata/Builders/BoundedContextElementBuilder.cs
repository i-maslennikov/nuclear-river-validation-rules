using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.River.Common.Metadata.Builders
{
    public sealed class BoundedContextElementBuilder : MetadataElementBuilder<BoundedContextElementBuilder, BoundedContextElement>
    {
        private const string ConceptualModelName = "ConceptualModel";
        private const string StoreModelName = "StoreModel";

        private readonly IDictionary<Uri, Uri> _entityMap = new Dictionary<Uri, Uri>();
        private string _name;
        private StructuralModelElementBuilder _conceptualModel;
        private StructuralModelElementBuilder _storeModel;

        public BoundedContextElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public BoundedContextElementBuilder ConceptualModel(StructuralModelElementBuilder modelElement)
        {
            _conceptualModel = modelElement.Name(ConceptualModelName);
            return this;
        }

        public BoundedContextElementBuilder StoreModel(StructuralModelElementBuilder modelElement)
        {
            _storeModel = modelElement.Name(StoreModelName);
            return this;
        }

        public BoundedContextElementBuilder Map(string conceptualEntityName, string storeEntityName)
        {
            _entityMap.Add(conceptualEntityName.AsUri(), storeEntityName.AsUri());
            return this;
        }

        protected override BoundedContextElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The context name was not specified.");
            }

            Uri uri = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<QueryingMetadataIdentity>(_name);
            var contextId = uri.AsIdentity();

            StructuralModelElement conceptualModel = null;
            StructuralModelElement storeModel = null;

            if (_conceptualModel != null)
            {
                conceptualModel = _conceptualModel;
                Childs(conceptualModel);
            }

            if (_storeModel != null)
            {
                storeModel = _storeModel;
                Childs(storeModel);
            }

            ProcessMappings(conceptualModel, storeModel);

            return new BoundedContextElement(contextId, conceptualModel, storeModel, Features);
        }

        private void ProcessMappings(StructuralModelElement conceptualModel, StructuralModelElement storeModel)
        {
            if (conceptualModel == null || storeModel == null)
            {
                return;
            }

            var conceptualEntities = conceptualModel.Entities.ToDictionary(x => x.Identity.Id);
            var storeEntities = storeModel.Entities.ToDictionary(x => x.Identity.Id);

            foreach (var map in _entityMap)
            {
                EntityElement conceptualEntity;
                EntityElement storeEntity;
                if (!conceptualEntities.TryGetValue(map.Key, out conceptualEntity) || !storeEntities.TryGetValue(map.Value, out storeEntity))
                {
                    throw new InvalidOperationException("The entity mapping cannot be resolved.");
                }

                ((IMetadataElementUpdater)conceptualEntity).AddFeature(new ElementMappingFeature(storeEntity));
            }
        }
    }
}