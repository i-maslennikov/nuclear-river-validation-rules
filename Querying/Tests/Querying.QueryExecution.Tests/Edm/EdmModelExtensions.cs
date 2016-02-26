using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace NuClear.Querying.Edm.Tests.Edm
{
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
}