using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm.EF;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.Storage.EntityFramework;

namespace NuClear.Querying.Storage
{
    public class DbModelFactory : IEFDbModelFactory
    {
        private const string AnnotationKey = "EntityId";

        private readonly IMetadataProvider _metadataProvider;
        private readonly IClrTypeProvider _clrTypeProvider;

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

            _metadataProvider = metadataProvider;
            _clrTypeProvider = clrTypeProvider;
        }

        public DbCompiledModel Create(string containerName, DbConnection connection)
        {
            Uri contextId = Metadata.Id.For<QueryingMetadataIdentity>(containerName);

            var boundedContextElement = LookupContext(contextId);
            if (boundedContextElement == null)
            {
                return null;
            }

            var modelBuilder = SetupBuilder(boundedContextElement);

            var dbModel = modelBuilder.Build(connection);
            return dbModel.Compile();
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

        private DbModelBuilder SetupBuilder(BoundedContextElement context)
        {
            var builder = CreateModelBuilder(_metadataProvider);

            foreach (var entityElement in context.ConceptualModel.Entities)
            {
                ProcessEntity(builder, entityElement);
            }

            return builder;
        }

        private void ProcessEntity(DbModelBuilder builder, EntityElement entityElement)
        {
            var entityType = _clrTypeProvider.Get(entityElement.Identity);
            if (entityType == null)
            {
                return;
            }

            var configuration = builder.RegisterEntity(entityType);

            ConfigureEntity(configuration, entityElement);

            var tableElement = entityElement.MappedEntity;
            if (tableElement != null)
            {
                ConfigureTable(configuration, tableElement);
            }
        }

        private static void ConfigureEntity(TypeConventionConfiguration configuration, EntityElement entityElement)
        {
            // add annotation
            configuration.Configure(x => x.HasTableAnnotation(AnnotationKey, entityElement.Identity.Id));

            // update entity set name
            configuration.Configure(x => x.HasEntitySetName(entityElement.EntitySetName ?? entityElement.ResolveName()));

            // declare keys
            configuration.Configure(x => x.HasKey(entityElement.KeyProperties.Select(p => p.ResolveName())));

            foreach (var propertyElement in entityElement.Properties)
            {
                var propertyType = propertyElement.PropertyType;
                if (propertyType.TypeKind == StructuralModelTypeKind.Enum) continue;

                var propertyName = propertyElement.ResolveName();
                if (propertyElement.IsNullable)
                {
                    configuration.Configure(x => x.Property(propertyName).IsOptional());
                }
                else
                {
                    configuration.Configure(x => x.Property(propertyName).IsRequired());
                }
            }
        }

        private static void ConfigureTable(TypeConventionConfiguration configuration, EntityElement tableElement)
        {
            string schemaName;
            var tableName = tableElement.ResolveName();
            ParseTableName(ref tableName, out schemaName);

            configuration.Configure(x => x.ToTable(tableName, schemaName));
            configuration.Configure(x => x.HasTableAnnotation(AnnotationKey, tableElement.Identity.Id));
        }

        private static void ParseTableName(ref string tableName, out string schemaName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            schemaName = null;

            var index = tableName.IndexOf('.');
            if (index >= 0)
            {
                schemaName = tableName.Substring(0, index);
                tableName = tableName.Substring(index + 1);
            }
        }

        private static DbModelBuilder CreateModelBuilder(IMetadataProvider metadata)
        {
            var builder = new DbModelBuilder();

            // conceptual model conventions
            builder.Conventions.Remove<PluralizingEntitySetNameConvention>();

            // store model conventions
            builder.Conventions.Remove<ForeignKeyIndexConvention>();
            builder.Conventions.Remove<PluralizingTableNameConvention>();

            // add custom conventions
            builder.Conventions.Add(new ForeignKeyMappingConvention(metadata));

            return builder;
        }

        private class ForeignKeyMappingConvention : IStoreModelConvention<AssociationType>
        {
            private const string AnnotationUri = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation" + ":" + AnnotationKey;

            private readonly IMetadataProvider _provider;

            public ForeignKeyMappingConvention(IMetadataProvider provider)
            {
                _provider = provider;
            }

            public void Apply(AssociationType item, DbModel model)
            {
                if (item.IsForeignKey && item.Constraint != null)
                {
                    var sourceId = ResolveId(item.Constraint.FromRole.GetEntityType());
                    var targetId = ResolveId(item.Constraint.ToRole.GetEntityType());

                    if (sourceId != null & targetId != null)
                    {
                        var sourceElement = LookupEntity(sourceId);
                        var targetElement = LookupEntity(targetId);

                        if (sourceElement != null & targetElement != null)
                        {
                            var relation = targetElement.Relations.FirstOrDefault(x => x.Target == sourceElement);
                            if (relation != null)
                            {
                                item.Constraint.ToProperties.Single().Name = relation.ResolveName();
                            }
                        }
                    }
                }
            }

            private EntityElement LookupEntity(Uri entityUrl)
            {
                EntityElement entityElement;
                return _provider.TryGetMetadata(entityUrl, out entityElement) ? entityElement : null;
            }

            private static Uri ResolveId(EntityType entityType)
            {
                MetadataProperty property;
                if (entityType.MetadataProperties.TryGetValue(AnnotationUri, false, out property))
                {
                    return (Uri)property.Value;
                }

                return null;
            }
        }
    }
}