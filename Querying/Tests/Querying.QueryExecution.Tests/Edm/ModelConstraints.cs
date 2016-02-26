using System;
using System.Collections.Generic;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;

using NUnit.Framework.Constraints;

namespace NuClear.Querying.Edm.Tests.Edm
{
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
                    throw new ArgumentException("The specified actual value is not a model.", nameof(actual));
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