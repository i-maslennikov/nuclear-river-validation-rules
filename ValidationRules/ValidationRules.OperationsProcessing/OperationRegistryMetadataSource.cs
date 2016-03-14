using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.ValidationRules.Domain.EntityTypes;
using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.Operations;

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
                    .Allow<CreateIdentity, EntityTypeOrder>()
                    .Allow<CreateIdentity, EntityTypeOrderPosition>()
                    .Allow<CreateIdentity, EntityTypeProject>()

                    .Allow<UpdateIdentity, EntityTypeOrder>()
                    .Allow<UpdateIdentity, EntityTypeOrderPosition>()
                    .Allow<UpdateIdentity, EntityTypeProject>()

                    .Allow<DeleteIdentity, EntityTypeOrderPosition>()

                    .Allow<AssignIdentity, EntityTypeOrder>()

                    .Allow<PublishGlobalAssociatedDeniedRulesIdentity>()

                    .Ignore<ManageGlobalAssociatedDeniedDraftRulesIdentity>()
                };

            Metadata = metadataElements.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}