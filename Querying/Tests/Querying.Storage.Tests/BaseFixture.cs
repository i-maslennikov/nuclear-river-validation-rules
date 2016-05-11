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
using NuClear.Querying.Edm.Emit;
using NuClear.Querying.Metadata.Builders;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Identities;
using NuClear.Querying.Storage;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Querying.Storage.Tests
{
    public class BaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        protected static DbProviderInfo EffortProvider => new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);

        protected static DbModel BuildModel(BoundedContextElement context)
        {
            var metadataProvider = CreateMetadataProvider(MockSource(context));
            var clrTypeProvider = new EmitClrTypeResolver(metadataProvider);
            clrTypeProvider.Build();

            var configurator = new DbModelBuilderConfigurator(metadataProvider, clrTypeProvider);

            var builder = configurator.Configure(context);
            var model = builder.Build(EffortProvider);

            model.Dump();

            return model;
        }

        protected static EdmModel BuildConceptualModel(BoundedContextElement context)
        {
            var model = BuildModel(context);

            model.ConceptualModel.Dump(EdmxExtensions.EdmModelType.Conceptual);

            return model.ConceptualModel;
        }

        protected static EdmModel BuildStoreModel(BoundedContextElement context)
        {
            var model = BuildModel(context);

            model.StoreModel.Dump(EdmxExtensions.EdmModelType.Store);

            return model.StoreModel;
        }

        protected static class ConceptualModel
        {
            public static Constraint IsValid => new ModelValidationConstraint();

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", nameof(value));
                    }

                    return model.IsValidCsdl(out _errors);
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
            public static Constraint IsValid => new ModelValidationConstraint();

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", nameof(value));
                    }

                    return model.IsValidSsdl(out _errors);
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
                return x => names.OrderBy(_ => _).SequenceEqual(x.EnumType.Members.Select(m => m.Name).OrderBy(_ => _));
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


        protected static IMetadataProvider CreateMetadataProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new QueryingMetadataIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { Metadata.Id.For<QueryingMetadataIdentity>(), context } });

            return source.Object;
        }
    }
}