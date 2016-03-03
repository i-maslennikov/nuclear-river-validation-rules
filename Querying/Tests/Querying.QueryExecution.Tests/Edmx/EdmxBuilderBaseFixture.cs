using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Effort.Provider;

using Moq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Querying.Edm.EF;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NuClear.Querying.Edm.Tests.Edmx
{
    public class EdmxBuilderBaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }


        protected static DbProviderInfo EffortProvider
        {
            get
            {
                return new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
            }
        }

        protected static DbModel BuildModel(BoundedContextElement context, IClrTypeBuilder clrTypeBuilder = null)
        {
            var builder = CreateBuilder(CreateMetadataProvider(MockSource(context)), clrTypeBuilder);
            var contextId = context.Identity.Id;
            var model = builder.Build(contextId, EffortProvider);

            model.Dump();

            return model;
        }

        protected static EdmModel BuildConceptualModel(BoundedContextElement context)
        {
            var model = BuildModel(context);

            EdmxExtensions.Dump(model.ConceptualModel, EdmxExtensions.EdmModelType.Conceptual);

            return model.ConceptualModel;
        }

        protected static EdmModel BuildStoreModel(BoundedContextElement context)
        {
            var model = BuildModel(context);

            EdmxExtensions.Dump(model.StoreModel, EdmxExtensions.EdmModelType.Store);

            return model.StoreModel;
        }

        protected static class ConceptualModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return EdmxExtensions.IsValidCsdl(model, out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }

                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class StoreModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return EdmxExtensions.IsValidSsdl(model, out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }
                    
                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class Property
        {
            public static Predicate<EdmProperty> OfType(PrimitiveTypeKind typeKind)
            {
                return x => x.PrimitiveType.PrimitiveTypeKind == typeKind;
            }

            public static Predicate<EdmProperty> IsNullable()
            {
                return x => x.Nullable;
            }

            public static Predicate<EdmProperty> IsKey()
            {
                return x => (x.DeclaringType as EntityType) != null && ((EntityType)x.DeclaringType).KeyMembers.Contains(x);
            }

            public static Predicate<EdmProperty> Members(params string[] names)
            {
                return x => names.OrderBy(_ => _).SequenceEqual(Enumerable.OrderBy<string, string>(x.EnumType.Members.Select(m => m.Name), _ => _));
            }
        }

        #region Metadata Helpers

        protected static BoundedContextElementBuilder NewContext(string name)
        {
            return BoundedContextElement.Config.Name(name);
        }

        protected static StructuralModelElementBuilder NewModel(params EntityElementBuilder[] entities)
        {
            return StructuralModelElement.Config.Elements(entities);
        }

        protected static EntityElementBuilder NewEntity(string name, params EntityPropertyElementBuilder[] properties)
        {
            var config = EntityElement.Config.Name(name);

            if (properties.Length == 0)
            {
                config.Property(NewProperty("Id")).HasKey("Id");
            }

            foreach (var propertyElementBuilder in properties)
            {
                config.Property(propertyElementBuilder);
            }

            return config;
        }

        protected static EntityPropertyElementBuilder NewProperty(string propertyName, ElementaryTypeKind propertyType = ElementaryTypeKind.Int64)
        {
            return EntityPropertyElement.Config.Name(propertyName).OfType(propertyType);
        }

        protected static EntityRelationElementBuilder NewRelation(string relationName)
        {
            return EntityRelationElement.Config.Name(relationName);
        }

        #endregion

        #region Utils

        private static EdmxModelBuilder CreateBuilder(IMetadataProvider metadataProvider, IClrTypeBuilder clrTypeBuilder = null)
        {
            return clrTypeBuilder == null 
                ? new EdmxModelBuilder(metadataProvider)
                : new EdmxModelBuilder(metadataProvider, clrTypeBuilder);
        }

        protected static IMetadataProvider CreateMetadataProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new QueryingMetadataIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { Metamodeling.Elements.Identities.Builder.Metadata.Id.For<QueryingMetadataIdentity>(), context } });

            return source.Object;
        }

        #endregion
    }
}