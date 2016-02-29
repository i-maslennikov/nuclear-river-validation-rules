using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    // todo: идентичен ValueObjectMetadata, подумать об удалении
    public class StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey> : MetadataElement<StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey>, StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey>>
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(TStatisticsObject).Name, UriKind.Relative).AsIdentity();

        public StatisticsRecalculationMetadata(
            MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> mapSpecificationProviderForTarget,
            Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> findSpecificationProvider)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
            FindSpecificationProvider = findSpecificationProvider;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> MapSpecificationProviderForTarget { get; private set; }

        public Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> FindSpecificationProvider { get; private set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}