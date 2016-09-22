using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public static class ArrangeMetadataElementBuilderExtensions
    {
        public static ArrangeMetadataElementBuilder Message(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Messages, data));

        public static ArrangeMetadataElementBuilder Aggregate(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Aggregates, data));

        public static ArrangeMetadataElementBuilder Fact(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Facts, data));

        public static ArrangeMetadataElementBuilder Erm(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Erm, data));

    }
}
