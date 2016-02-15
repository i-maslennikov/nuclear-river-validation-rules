using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Features;

namespace NuClear.River.Common.Metadata.Builders
{
    public sealed class EntityElementBuilder : MetadataElementBuilder<EntityElementBuilder, EntityElement>
    {
        private readonly HashSet<Uri> _keyNames = new HashSet<Uri>();
        private readonly List<EntityPropertyElementBuilder> _propertyConfigs = new List<EntityPropertyElementBuilder>();
        private readonly List<EntityRelationElementBuilder> _relationConfigs = new List<EntityRelationElementBuilder>();

        private Uri _entityId;
        private string _entitySetName;

        public Uri EntityId
        {
            get
            {
                if (_entityId == null)
                {
                    throw new InvalidOperationException("The id was not set.");
                }
                return _entityId;
            }
        }

        public IReadOnlyCollection<EntityPropertyElementBuilder> PropertyConfigs
        {
            get
            {
                return _propertyConfigs;
            }
        }

        public IReadOnlyCollection<EntityRelationElementBuilder> RelationConfigs
        {
            get
            {
                return _relationConfigs;
            }
        }

        public EntityElementBuilder Name(string name)
        {
            _entityId = UriExtensions.AsUri(name);
            return this;
        }

        public EntityElementBuilder EntitySetName(string entitySetName)
        {
            _entitySetName = entitySetName;
            return this;
        }

        public EntityElementBuilder HasKey(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                _keyNames.Add(UriExtensions.AsUri(propertyName));
            }
            return this;
        }

        public EntityElementBuilder Property(EntityPropertyElementBuilder property)
        {
            _propertyConfigs.Add(property);
            return this;
        }

        public EntityElementBuilder Relation(EntityRelationElementBuilder relation)
        {
            _relationConfigs.Add(relation);
            return this;
        }

        protected override EntityElement Create()
        {
            if (_entityId == null)
            {
                throw new InvalidOperationException("The entity name was not specified.");
            }

            if (!string.IsNullOrWhiteSpace(_entitySetName))
            {
                AddFeatures(new EntitySetFeature(_entitySetName));
            }

            ProcessProperties();
            ProcessRelations();

            return new EntityElement(_entityId.AsIdentity(), Features);
        }

        private void ProcessProperties()
        {
            var keys = new List<EntityPropertyElement>();

            foreach (var propertyElement in _propertyConfigs.Select(x => (EntityPropertyElement)x))
            {
                Childs(propertyElement);

                if (_keyNames.Contains(propertyElement.Identity.Id))
                {
                    keys.Add(propertyElement);
                }
            }

            if (keys.Count > 0)
            {
                AddFeatures(new EntityIdentityFeature(keys));
            }
        }

        private void ProcessRelations()
        {
            Childs(_relationConfigs.Select(x => (IMetadataElement)(EntityRelationElement)x).ToArray());
        }
    }
}