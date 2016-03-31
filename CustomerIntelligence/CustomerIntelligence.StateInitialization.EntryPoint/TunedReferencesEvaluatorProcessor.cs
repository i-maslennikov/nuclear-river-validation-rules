using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.References;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    class TunedReferencesEvaluatorProcessor : IMetadataProcessor
    {
        public IMetadataKindIdentity[] TargetMetadataConstraints
        {
            get { return new IMetadataKindIdentity[0]; }
        }

        public void Process(
            IMetadataKindIdentity metadataKind,
            MetadataSet flattenedMetadata,
            MetadataSet concreteKindMetadata,
            IMetadataElement element)
        {
            bool hasReferences = false;
            var dereferencedChilds = new List<IMetadataElement>();
            foreach (var child in element.Elements)
            {
                var reference = child as MetadataReference;
                if (reference == null)
                {
                    dereferencedChilds.Add(child);
                    continue;
                }

                IMetadataElement metadataElement;
                var absoluteId = reference.ReferencedElementId.IsAbsoluteUri
                                     ? reference.ReferencedElementId
                                     : element.Identity.Id.WithRelative(reference.ReferencedElementId);

                if (!flattenedMetadata.Metadata.TryGetValue(absoluteId, out metadataElement))
                {
                    throw new InvalidOperationException("Can't resolve metadata for referenced element : " + reference.ReferencedElementId + ". References is ecounterred in " + reference.Parent.Identity.Id + " childs list");
                }

                hasReferences = true;
                flattenedMetadata.Metadata.Remove(reference.Identity.Id);
                concreteKindMetadata.Metadata.Remove(reference.Identity.Id);
                ((IMetadataElementUpdater)metadataElement).ReferencedBy(element);
                dereferencedChilds.Add(metadataElement);
            }

            if (!hasReferences)
            {
                return;
            }

            ((IMetadataElementUpdater)element).ReplaceChilds(dereferencedChilds);
        }
    }
}
