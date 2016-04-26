using System;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Features;

namespace NuClear.Querying.Metadata.Builders
{
    public sealed class EntityPropertyElementBuilder : MetadataElementBuilder<EntityPropertyElementBuilder, EntityPropertyElement>
    {
        private string _name;
        private bool _isNullable;
        private IStructuralModelTypeElement _typeElement;
        private Uri _typeReference;

        public IStructuralModelTypeElement TypeElement
        {
            get
            {
                return _typeElement;
            }
        }

        public Uri TypeReference
        {
            get
            {
                return _typeReference;
            }
        }

        public EntityPropertyElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityPropertyElementBuilder OfType(ElementaryTypeKind elementaryTypeKind)
        {
            _typeElement = PrimitiveTypeElement.OfKind(elementaryTypeKind);
            _typeReference = TypeElement.Identity.Id;
            return this;
        }

        public EntityPropertyElementBuilder OfType<T>(T typeElement) where T : IStructuralModelTypeElement
        {
            _typeElement = typeElement;
            _typeReference = TypeElement.Identity.Id;
            return this;
        }

        public EntityPropertyElementBuilder Nullable()
        {
            _isNullable = true;
            return this;
        }

        protected override EntityPropertyElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The property name was not specified.");
            }
            
            if (_typeElement == null)
            {
                throw new InvalidOperationException("The property type was not specified");
            }

            if (_isNullable)
            {
                AddFeatures(new EntityPropertyNullableFeature(true));
            }

            return new EntityPropertyElement(UriExtensions.AsUri(_name).AsIdentity(), _typeElement, Features);
        }
   }
}