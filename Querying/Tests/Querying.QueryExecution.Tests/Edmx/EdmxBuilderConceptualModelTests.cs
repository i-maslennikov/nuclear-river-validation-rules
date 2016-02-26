using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;

using NUnit.Framework;

namespace NuClear.Querying.Edm.Tests.Edmx
{
    [TestFixture]
    internal class EdmxBuilderConceptualModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldExposeEntitySets()
        {
            var config = NewContext("Library").ConceptualModel(
                NewModel(
                    NewEntity("Book"), 
                    NewEntity("Author").EntitySetName("Authors")));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.FindEntitySet("Authors"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeRelatedEntitySet()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book").Relation(NewRelation("Authors").DirectTo(NewEntity("Author")).AsMany())));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldNotExposeAssociationSet()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book"), NewEntity("Author").EntitySetName("Authors")));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.Container.AssociationSets, Has.Count.EqualTo(0));
        }

        [Test]
        public void ShouldExposeAssociationSet()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book").Relation(NewRelation("Authors").DirectTo(NewEntity("Author")).AsMany())));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.Container.AssociationSets, Has.Count.EqualTo(1));
            Assert.That(model.FindAssociationSet("Book_Authors"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.EntityTypes, Has.Count.EqualTo(2));
            Assert.That(model.FindEntityType("Book"), Is.Not.Null);
            Assert.That(model.FindEntityType("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeRelatedEntityType()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book").Relation(NewRelation("Authors").DirectTo(NewEntity("Author")).AsMany())));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.EntityTypes, Has.Count.EqualTo(2));
            Assert.That(model.FindEntityType("Book"), Is.Not.Null);
            Assert.That(model.FindEntityType("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeRelation()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book").Relation(NewRelation("Authors").DirectTo(NewEntity("Author")).AsMany())));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.AssociationTypes, Has.Count.EqualTo(1));

            var associationType = model.FindAssociationType("Book_Authors");
            Assert.That(associationType, Is.Not.Null);

            var sourceEnd = associationType.KeyMembers.FirstOrDefault() as AssociationEndMember;
            Assert.That(sourceEnd, Is.Not.Null);
            Assert.That(sourceEnd.RelationshipMultiplicity, Is.EqualTo(RelationshipMultiplicity.ZeroOrOne));
            Assert.That(sourceEnd.GetEntityType().Name, Is.EqualTo("Book"));

            var targetEnd = associationType.KeyMembers.LastOrDefault() as AssociationEndMember;
            Assert.That(targetEnd, Is.Not.Null);
            Assert.That(targetEnd.RelationshipMultiplicity, Is.EqualTo(RelationshipMultiplicity.Many));
            Assert.That(targetEnd.GetEntityType().Name, Is.EqualTo("Author"));
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = NewContext("Context").ConceptualModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Byte))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))));

            var model = BuildConceptualModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));

            var entityType = model.FindEntityType("Entity");
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.Byte)));
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.String)));
        }

        [Test]
        public void ShouldExposeEntityKeys()
        {
            var config = NewContext("Context").ConceptualModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").HasKey("Id", "Name")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Byte))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))));

            var model = BuildConceptualModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));

            var entityType = model.FindEntityType("Entity");
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.IsKey()));
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null.And.Matches(Property.IsKey()));
        }

        [Test]
        public void ShouldSupportNullable()
        {
            var config = NewContext("Context").ConceptualModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Byte))
                        .Property(EntityPropertyElement.Config.Name("NonNullable").OfType(ElementaryTypeKind.String))
                        .Property(EntityPropertyElement.Config.Name("Nullable").OfType(ElementaryTypeKind.String).Nullable())));

            var model = BuildConceptualModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));

            var entityType = model.FindEntityType("Entity");
            Assert.NotNull(entityType);

            Assert.That(entityType.FindProperty("Nullable"), Is.Not.Null.And.Matches<EdmProperty>(x => x.Nullable));
            Assert.That(entityType.FindProperty("NonNullable"), Is.Not.Null.And.Not.Matches<EdmProperty>(x => x.Nullable));
        }

        [Test]
        public void ShouldSupportAllPrimitiveTypes()
        {
            var primitiveTypes = Enum.GetValues(typeof(ElementaryTypeKind)).OfType<ElementaryTypeKind>().ToArray();

            var element = EntityElement.Config.Name("Entity").HasKey("PropertyOfInt32");
            foreach (var propertyType in primitiveTypes)
            {
                element.Property(NewProperty("PropertyOf" + propertyType.ToString("G")).OfType(propertyType));
            }

            var model = BuildConceptualModel(NewContext("Context").ConceptualModel(NewModel(element)));
            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));

            var entity = model.FindEntityType("Entity");
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count);
        }

        [Test]
        public void ShouldSupportEnumType()
        {
            var config = NewContext("Context")
                .ConceptualModel(
                    NewModel(
                        NewEntity("Person")
                            .Property(EntityPropertyElement.Config.Name("Gender").OfType<EnumTypeElement>(EnumTypeElement.Config.Name("GenderEnum")))
                            .Property(EntityPropertyElement.Config.Name("NullableGender").OfType<EnumTypeElement>(EnumTypeElement.Config.Name("GenderEnum")).Nullable()))
                .Types<EnumTypeElement>(EnumTypeElement.Config.Name("GenderEnum").Member("Male", 1).Member("Female", 2)));

            var model = BuildConceptualModel(config);
            Assert.NotNull(model);

            var entity = model.FindEntityType("Person");
            Assert.NotNull(entity);
            Assert.That(entity.FindProperty("Gender"), Is.Not.Null.And.Matches(Property.Members("Male", "Female")));
            Assert.That(entity.FindProperty("NullableGender"), Is.Not.Null.And.Matches(Property.IsNullable()));
        }
    }
}
