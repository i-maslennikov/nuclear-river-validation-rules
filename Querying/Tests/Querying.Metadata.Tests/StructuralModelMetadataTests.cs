using System;
using System.Collections;
using System.Linq;

using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Elements;

using NUnit.Framework;

namespace NuClear.QueryingMetadata.Tests
{
    [TestFixture]
    internal class StructuralModelMetadataTests : BaseMetadataFixture<StructuralModelElement, StructuralModelElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(StructuralModelElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(StructuralModelElement.Config.Name("Model"), MetadataKind.Identity)
                    .Returns("{'Identity':{'Id':'Model'}}")
                    .SetName("ShouldDeclareModel");
                yield return Case(StructuralModelElement.Config.Name("Model"), MetadataKind.Elements | MetadataKind.Features)
                    .Returns("{'Features':[],'Elements':[]}")
                    .SetName("ShouldDeclareEmptyModel");
                yield return Case(
                    StructuralModelElement.Config.Name("Model")
                    .Elements(
                        EntityElement.Config.Name("Book").Relation(EntityRelationElement.Config.Name("Authors").DirectTo(EntityElement.Config.Name("Author")).AsMany())
                    ), 
                    MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'Model'},'Elements':[{'Identity':{'Id':'Author'},'Elements':[]},{'Identity':{'Id':'Book'},'Elements':[{'Identity':{'Id':'Authors'},'Elements':[]}]}]}")
                    .SetName("ShouldProcessNestedEntity");
                yield return Case(
                    StructuralModelElement.Config.Name("Model")
                    .Elements(
                        EntityElement.Config.Name("Author"),
                        EntityElement.Config.Name("Book").Relation(EntityRelationElement.Config.Name("Authors").DirectTo(EntityElement.Config.Name("Author")).AsMany())
                    ), 
                    MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'Model'},'Elements':[{'Identity':{'Id':'Author'},'Elements':[]},{'Identity':{'Id':'Book'},'Elements':[{'Identity':{'Id':'Authors'},'Elements':[]}]}]}")
                    .SetName("ShouldProcessEntityLink");
            }
        }

        [Test]
        public void ShouldDistinguishRootAndEntities()
        {
            StructuralModelElement model = StructuralModelElement.Config.Name("Model")
                .Elements(
                    EntityElement.Config.Name("Book").Relation(
                        EntityRelationElement.Config.Name("Authors").DirectTo(EntityElement.Config.Name("Author")
                    ).AsMany()
                ));

            Assert.That(model.RootEntities.Count(), Is.EqualTo(1));
            Assert.That(model.Entities.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ShouldMapRoots()
        {
            StructuralModelElement model = StructuralModelElement.Config.Name("Model")
                .Elements(
                    EntityElement.Config.Name("Author"),
                    EntityElement.Config.Name("Book").Relation(EntityRelationElement.Config.Name("Authors").DirectTo(EntityElement.Config.Name("Author")).AsMany())
                );

            Assert.That(model.RootEntities.Count(), Is.EqualTo(2));
            Assert.That(model.Entities.Count(), Is.EqualTo(2));
        }
    }
}
