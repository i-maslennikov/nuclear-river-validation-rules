using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace NuClear.Replication.Bulk.API.Metadata
{
    public sealed class BulkReplicationMetadataElement : MetadataElement<BulkReplicationMetadataElement, BulkReplicationMetadataBuilder>
    {
        private IMetadataElementIdentity _identity;

        public BulkReplicationMetadataElement(string commandLineKey, IReadOnlyCollection<Type> dataObjectTypes, IReadOnlyCollection<string> essentialViewNames, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<BulkReplicationMetadataKindIdentity>(commandLineKey).Build().AsIdentity();
            DataObjectTypes = dataObjectTypes;
            EssentialViews = essentialViewNames;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public IReadOnlyCollection<Type> DataObjectTypes { get; }

        public IReadOnlyCollection<string> EssentialViews { get; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity) => _identity = actualMetadataElementIdentity;
    }
}
