using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class ImportDocumentMetadata<TDto> : MetadataElement<ImportDocumentMetadata<TDto>, ImportDocumentMetadataBuilder<TDto>>
    {
        private IMetadataElementIdentity _identity;

        public ImportDocumentMetadata(
            IEnumerable<IMetadataFeature> features) : base(features)
        {
            _identity = new Uri($"{typeof(TDto).Name}", UriKind.Relative).AsIdentity();
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override IMetadataElementIdentity Identity
            => _identity;

        public IMapSpecification<TDto, IReadOnlyCollection<IOperation>> RecalculationSpecification
            => Features.OfType<MapSpecificationFeature<TDto, IReadOnlyCollection<IOperation>>>().Single().MapSpecificationProvider;
    }
}