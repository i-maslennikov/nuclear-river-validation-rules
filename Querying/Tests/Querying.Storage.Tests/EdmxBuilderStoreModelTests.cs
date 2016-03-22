using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Elements;

using NUnit.Framework;

namespace Querying.Storage.Tests
{
    [TestFixture]
    internal class EdmxBuilderStoreModelTests : EdmxBuilderBaseFixture
    {
        private BoundedContextElementBuilder _config;

        [SetUp]
        public void Setup()
        {
            _config = NewContext("Library")
                .ConceptualModel(NewModel(
                    NewEntity("Book")
                        .Property(NewProperty("Title", ElementaryTypeKind.String))
                        .Property(NewProperty("Publisher", ElementaryTypeKind.String).Nullable()),
                    NewEntity("Author")
                        .Property(NewProperty("Name", ElementaryTypeKind.String))
                        .Relation(NewRelation("Books").DirectTo(NewEntity("Book")).AsMany())))
                .StoreModel(NewModel(
                    NewEntity("Catalog.Author")
                        .Property(NewProperty("Name", ElementaryTypeKind.String)),
                    NewEntity("Catalog.Book")
                        .Property(NewProperty("Title", ElementaryTypeKind.String))
                        .Property(NewProperty("Publisher", ElementaryTypeKind.String).Nullable())
                        .Relation(NewRelation("AuthorId").DirectTo(NewEntity("Catalog.Author")).AsOne())))
                .Map("Author", "Catalog.Author")
                .Map("Book", "Catalog.Book");
        }

        [Test]
        public void ShouldExposeEntitySets()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.EntityTypes, Has.Count.EqualTo(2));
            Assert.That(model.FindEntityType("Book"), Is.Not.Null);
            Assert.That(model.FindEntityType("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeAssociationSet()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.Container.AssociationSets, Has.Count.EqualTo(1));
            Assert.That(model.FindAssociationSet("Author_Books"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeAssociationType()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.AssociationTypes, Has.Count.EqualTo(1));

            var associationType = model.FindAssociationType("Author_Books");
            Assert.That(associationType, Is.Not.Null);

            var sourceEnd = associationType.KeyMembers.FirstOrDefault() as AssociationEndMember;
            Assert.That(sourceEnd, Is.Not.Null);
            Assert.That(sourceEnd.RelationshipMultiplicity, Is.EqualTo(RelationshipMultiplicity.ZeroOrOne));
            Assert.That(sourceEnd.GetEntityType().Name, Is.EqualTo("Author"));

            var targetEnd = associationType.KeyMembers.LastOrDefault() as AssociationEndMember;
            Assert.That(targetEnd, Is.Not.Null);
            Assert.That(targetEnd.RelationshipMultiplicity, Is.EqualTo(RelationshipMultiplicity.Many));
            Assert.That(targetEnd.GetEntityType().Name, Is.EqualTo("Book"));
        }

        [Test]
        public void ShouldSetCustomSchemaAndTableName()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entitySet = model.FindEntitySet("Book");
            Assert.That(entitySet, Is.Not.Null);
            Assert.That(entitySet.Name, Is.EqualTo("Book"));
            Assert.That(entitySet.Schema, Is.EqualTo("Catalog"));
            Assert.That(entitySet.Table, Is.EqualTo("Book"));
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindEntityType("Book");
            Assert.NotNull(entityType);

            Assert.AreEqual(4, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.Int64)));
            Assert.That(entityType.FindProperty("Title"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.String)));
            Assert.That(entityType.FindProperty("Publisher"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.String)));
            Assert.That(entityType.FindProperty("AuthorId"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.Int64)));
        }

        [Test]
        public void ShouldExposeEntityKeys()
        {
            var model = BuildStoreModel(_config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindEntityType("Book");
            Assert.NotNull(entityType);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.IsKey()));
            Assert.That(entityType.FindProperty("Title"), Is.Not.Null.And.Not.Matches(Property.IsKey()));
        }

        [Test]
        public void ShouldSupportNullable()
        {
            var model = BuildStoreModel(_config);
            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindEntityType("Book");
            Assert.NotNull(entityType);

            Assert.That(entityType.FindProperty("Title"), Is.Not.Null.And.Not.Matches<EdmProperty>(x => x.Nullable));
            Assert.That(entityType.FindProperty("Publisher"), Is.Not.Null.And.Matches<EdmProperty>(x => x.Nullable));
        }

        [Test]
        public void ShouldSupportAllPrimitiveTypes()
        {
            var primitiveTypes = Enum.GetValues(typeof(ElementaryTypeKind)).OfType<ElementaryTypeKind>().ToArray();

            var entity = EntityElement.Config.Name("Entity").HasKey("PropertyOfInt32");
            var table = EntityElement.Config.Name("Entity").HasKey("PropertyOfInt32");
            foreach (var propertyType in primitiveTypes)
            {
                entity.Property(NewProperty("PropertyOf" + propertyType.ToString("G")).OfType(propertyType));
                table.Property(NewProperty("PropertyOf" + propertyType.ToString("G")).OfType(propertyType));
            }

            var model = BuildStoreModel(
                NewContext("Context")
                    .ConceptualModel(NewModel(entity))
                    .StoreModel(NewModel(table))
                    .Map("Entity", "Entity"));

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var edmEntity = model.FindEntityType("Entity");
            Assert.NotNull(edmEntity);
            Assert.AreEqual(primitiveTypes.Length, edmEntity.DeclaredProperties.Count);
        }
   }
}