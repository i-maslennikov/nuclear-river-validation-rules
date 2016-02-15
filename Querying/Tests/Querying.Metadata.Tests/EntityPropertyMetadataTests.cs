using System;
using System.Collections;

using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Elements;

using NUnit.Framework;

namespace NuClear.QueryingMetadata.Tests
{
    [TestFixture]
    internal class EntityPropertyMetadataTests : BaseMetadataFixture<EntityPropertyElement, EntityPropertyElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(EntityPropertyElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(EntityPropertyElement.Config.Name("Property"))
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoType");
                yield return Case(EntityPropertyElement.Config.Name("Property").OfType(ElementaryTypeKind.String), "Identity", "PropertyType")
                    .Returns("{'PropertyType':{'Identity':{'Id':'erm://metadata/Querying/String'}},'Identity':{'Id':'Property'}}")
                    .SetName("ShouldDeclareProperty");
                yield return Case(EntityPropertyElement.Config.Name("Property").OfType(ElementaryTypeKind.Int32).Nullable(), "Identity", "PropertyType", "Features")
                    .Returns("{'PropertyType':{'Identity':{'Id':'erm://metadata/Querying/Int32'},'Features':[]},'Identity':{'Id':'Property'},'Features':[{'IsNullable':true}]}")
                    .SetName("ShouldDeclareNullableProperty");
                yield return Case(EntityPropertyElement.Config.Name("Property").OfType<EnumTypeElement>(EnumTypeElement.Config.Name("Gender").Member("Female", 1).Member("Male", 2)), "Identity", "PropertyType", "Members")
                    .Returns("{'PropertyType':{'Members':{'Female':1,'Male':2},'Identity':{'Id':'Gender'}},'Identity':{'Id':'Property'}}")
                    .SetName("ShouldDeclareEnumProperty");
            }
        }
    }
}
