using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public class StatisticsRecalculationMetadataIdentity : MetadataKindIdentityBase<StatisticsRecalculationMetadataIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "StatisticsRecalculation");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Statistics recalculation process descriptive metadata"; }
        }
    }
}