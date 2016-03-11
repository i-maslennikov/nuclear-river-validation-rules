using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.ValidationRules.Domain.EntityTypes;
using NuClear.ValidationRules.OperationsProcessing.Contexts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class OperationRegistryMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        public OperationRegistryMetadataSource()
        {
            var metadataElements = new OperationRegistryMetadataElement[]
                {
                    OperationRegistryMetadataElement
                    .Config
                    .For<FactsSubDomain>()
                    .AllowedOperationIdentities(new HashSet<StrictOperationIdentity>
                    {
                        CreateIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeProject.Instance),

                        UpdateIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeProject.Instance),

                        DeleteIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),

                        AssignIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                    })
                };


            Metadata = metadataElements.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}