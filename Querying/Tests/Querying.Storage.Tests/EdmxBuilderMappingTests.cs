using NUnit.Framework;

namespace Querying.Storage.Tests
{
    [TestFixture]
    internal class EdmxBuilderMappingTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldMapEntityOnTable()
        {
            var config = NewContext("Library")
                .ConceptualModel(NewModel(NewEntity("Book")))
                .StoreModel(NewModel(NewEntity("StoreSchema.BookTable")))
                .Map("Book", "StoreSchema.BookTable");

            var model = BuildModel(config);
            Assert.That(model, Is.Not.Null);

            var conceptualEntity = model.ConceptualModel.FindEntitySet("Book");
            Assert.That(conceptualEntity, Is.Not.Null);
            Assert.That(conceptualEntity.Schema, Is.Null);
            Assert.That(conceptualEntity.Table, Is.Null);

            var storeEntity = model.StoreModel.FindEntitySet("Book");
            Assert.That(storeEntity, Is.Not.Null);
            Assert.That(storeEntity.Schema, Is.EqualTo("StoreSchema"));
            Assert.That(storeEntity.Table, Is.EqualTo("BookTable"));
        }
    }
}