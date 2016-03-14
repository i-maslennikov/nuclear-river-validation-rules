using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class StatisticsRecalculationMetadataIdentity : MetadataKindIdentityBase<StatisticsRecalculationMetadataIdentity>
    {
        public override Uri Id => MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "StatisticsRecalculation");

        public override string Description => "Statistics recalculation process descriptive metadata";
    }
}