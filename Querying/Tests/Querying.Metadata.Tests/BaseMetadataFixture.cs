using System;
using System.Collections;

using NuClear.Metamodeling.Elements;

using NUnit.Framework;

namespace NuClear.Querying.Metadata.Metadata.Tests
{
    internal abstract class BaseMetadataFixture<TMetadataElement, TBuilder>
        where TMetadataElement : MetadataElement
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
    {
        public abstract IEnumerable TestCases { get; }

        [Test, TestCaseSource("TestCases")]
        public string CheckSerialization(Func<string> actor)
        {
            return actor();
        }

        protected static TestCaseData Case(TBuilder config)
        {
            return Case(config, MetadataKind.Identity | MetadataKind.Features);
        }

        protected static TestCaseData Case(TBuilder config, MetadataKind properties)
        {
            return new TestCaseData((Func<string>)(() => SerializeAndDump(config, properties)));
        }

        protected static TestCaseData Case(TBuilder config, params string[] properties)
        {
            return new TestCaseData((Func<string>)(() => SerializeAndDump(config, properties)));
        }

        private static string SerializeAndDump(TMetadataElement element, MetadataKind metadata)
        {
            element.Dump(metadata);

            return element.Serialize(metadata);
        }

        private static string SerializeAndDump(TMetadataElement element, string[] properties)
        {
            element.Dump(properties);

            return element.Serialize(properties);
        }
   }
}