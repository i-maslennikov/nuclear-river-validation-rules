using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.River.Common.Metadata.Builders
{
    public sealed class OperationRegistryMetadataElementBuilder : MetadataElementBuilder<OperationRegistryMetadataElementBuilder, OperationRegistryMetadataElement>
    {
        private string[] _segments;

        protected override OperationRegistryMetadataElement Create()
        {
            var identity = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<OperationRegistryMetadataIdentity>(_segments).Build().AsIdentity();

            return new OperationRegistryMetadataElement(identity, Features);
        }

        public OperationRegistryMetadataElementBuilder For<TSubDomain>()
            where TSubDomain : ISubDomain
        {
            _segments = new[] { typeof(TSubDomain).Name };
            return this;
        }

        public OperationRegistryMetadataElementBuilder Allow<TOperation>()
            where TOperation : OperationIdentityBase<TOperation>, INonCoupledOperationIdentity, new()
        {
            var operationIdentity = OperationIdentityBase<TOperation>.Instance.NonCoupled();
            AddFeatures(new OperationRegistryMetadataElement.AllowedOperationFeature(operationIdentity));
            return this;
        }

        public OperationRegistryMetadataElementBuilder Allow<TOperation, TEntity>()
           where TOperation : OperationIdentityBase<TOperation>, IEntitySpecificOperationIdentity, new()
           where TEntity : EntityTypeBase<TEntity>, IEntityType, new()
        {
            var operationIdentity = OperationIdentityBase<TOperation>.Instance.SpecificFor(EntityTypeBase<TEntity>.Instance);
            AddFeatures(new OperationRegistryMetadataElement.AllowedOperationFeature(operationIdentity));
            return this;
        }

        public OperationRegistryMetadataElementBuilder Allow<TOperation, TEntity1, TEntity2>()
           where TOperation : OperationIdentityBase<TOperation>, IEntitySpecificOperationIdentity, new()
           where TEntity1 : EntityTypeBase<TEntity1>, IEntityType, new()
           where TEntity2 : EntityTypeBase<TEntity2>, IEntityType, new()
        {
            var operationIdentity = OperationIdentityBase<TOperation>.Instance.SpecificFor(EntityTypeBase<TEntity1>.Instance, EntityTypeBase<TEntity2>.Instance);
            AddFeatures(new OperationRegistryMetadataElement.AllowedOperationFeature(operationIdentity));
            return this;
        }

        public OperationRegistryMetadataElementBuilder Ignore<TOperation, TEntity>()
            where TOperation : OperationIdentityBase<TOperation>, IEntitySpecificOperationIdentity, new()
            where TEntity : EntityTypeBase<TEntity>, IEntityType, new()
        {
            var operationIdentity = OperationIdentityBase<TOperation>.Instance.SpecificFor(EntityTypeBase<TEntity>.Instance);
            AddFeatures(new OperationRegistryMetadataElement.IgnoredOperationFeature(operationIdentity));
            return this;
        }

        public OperationRegistryMetadataElementBuilder Ignore<TOperation>()
            where TOperation : OperationIdentityBase<TOperation>, INonCoupledOperationIdentity, new()
        {
            var operationIdentity = OperationIdentityBase<TOperation>.Instance.NonCoupled();
            AddFeatures(new OperationRegistryMetadataElement.IgnoredOperationFeature(operationIdentity));
            return this;
        }
    }
}