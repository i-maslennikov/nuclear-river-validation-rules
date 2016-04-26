using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm.EF;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Identities;
using NuClear.Storage.EntityFramework;

namespace NuClear.Querying.Storage
{
    public class DbModelFactory : IEFDbModelFactory
    {
        private readonly DbModelBuilderConfigurator _dbModelBuilderConfigurator;
        private readonly IMetadataProvider _metadataProvider;

        private readonly Dictionary<Uri, DbCompiledModel> _modelsCache = new Dictionary<Uri, DbCompiledModel>();

        public DbModelFactory(IMetadataProvider metadataProvider, IClrTypeProvider clrTypeProvider)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException(nameof(metadataProvider));
            }
            if (clrTypeProvider == null)
            {
                throw new ArgumentNullException(nameof(clrTypeProvider));
            }

            _dbModelBuilderConfigurator = new DbModelBuilderConfigurator(metadataProvider, clrTypeProvider);
            _metadataProvider = metadataProvider;
        }

        public DbCompiledModel Create(string containerName, DbConnection connection)
        {
            Uri contextId = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<QueryingMetadataIdentity>(containerName);

            DbCompiledModel compiledModel;
            if (_modelsCache.TryGetValue(contextId, out compiledModel))
            {
                return compiledModel;
            }

            var boundedContextElement = LookupContext(contextId);
            if (boundedContextElement == null)
            {
                return null;
            }

            var modelBuilder = _dbModelBuilderConfigurator.Configure(boundedContextElement);

            compiledModel = modelBuilder.Build(connection).Compile();
            _modelsCache.Add(contextId, compiledModel);

            return compiledModel;
        }

        private BoundedContextElement LookupContext(Uri contextUrl)
        {
            BoundedContextElement boundedContextElement;
            _metadataProvider.TryGetMetadata(contextUrl, out boundedContextElement);
            if (boundedContextElement?.ConceptualModel == null)
            {
                return null;
            }

            return boundedContextElement;
        }
    }
}