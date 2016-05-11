using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Querying.Edm;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Identities;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class EdmModelBuilderTests
    {
        [Test]
        public void ShouldBuildValidModelForCustomerIntelligenceContext()
        {
            var contextId = Metadata.Id.For<QueryingMetadataIdentity>("CustomerIntelligence");

            BoundedContextElement boundedContext;
            TestMetadataProvider.Instance.TryGetMetadata(contextId, out boundedContext);

            var model = BuildModel(contextId);

            Assert.That(model, Is.Not.Null.And.Matches(ModelConstraints.IsValid));
        }

        private static IEdmModel BuildModel(Uri contextId)
        {
            var builder = new EdmModelBuilder(TestMetadataProvider.Instance, new NullEdmModelAnnotator());
            var model = builder.Build()[contextId];

            model.Dump();

            return model;
        }
    }

    internal static class EdmModelExtensions
    {
        [Conditional("DEBUG")]
        public static void Dump(this IEdmModel model)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
            {
                IEnumerable<EdmError> errors;
                EdmxWriter.TryWriteEdmx(model, writer, EdmxTarget.OData, out errors);
            }

            Debug.WriteLine(sb.ToString());
        }
    }

    internal static class ModelConstraints
    {
        public static Constraint IsValid => new ModelValidationConstraint();

        private class ModelValidationConstraint : Constraint
        {
            private const int MaxErrorsToDisplay = 5;
            private IEnumerable<EdmError> _errors;

            public override bool Matches(object actual)
            {
                var model = actual as IEdmModel;
                if (model == null)
                {
                    throw new ArgumentException("The specified actual value is not a model.", "actual");
                }

                return model.Validate(out _errors);
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WriteCollectionElements(_errors, 0, MaxErrorsToDisplay);
            }
        }
    }
}