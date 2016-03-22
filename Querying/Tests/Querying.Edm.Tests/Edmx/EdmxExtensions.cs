using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NuClear.Querying.Edm.Tests.Edmx
{
    internal static class EdmxExtensions
    {
        [Conditional("DEBUG")]
        public static void Dump(this DbModel model)
        {
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                EdmxWriter.WriteEdmx(model, xmlWriter);
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        [Conditional("DEBUG")]
        public static void Dump(this EdmModel edmModel, EdmModelType modelType)
        {
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true }))
            {
                switch (modelType)
                {
                    case EdmModelType.Conceptual:
                        SerializeAndValidateCsdl(edmModel, xmlWriter);
                        break;
                    case EdmModelType.Store:
                        SerializeAndValidateSsdl(edmModel, xmlWriter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("modelType");
                }
            }

            Debug.WriteLine(stringBuilder.ToString());
        }

        public static bool IsValidCsdl(this EdmModel model, out IReadOnlyCollection<string> errors)
        {
            var writer = XmlWriter.Create(new StreamWriter(Stream.Null));

            errors = SerializeAndValidateCsdl(model, writer).Select(ToText).ToArray();

            return errors.Count == 0;
        }

        public static bool IsValidSsdl(this EdmModel model, out IReadOnlyCollection<string> errors)
        {
            var writer = XmlWriter.Create(new StreamWriter(Stream.Null));

            errors = SerializeAndValidateSsdl(model, writer).Select(ToText).ToArray();

            return errors.Count == 0;
        }

        private static List<DataModelErrorEventArgs> SerializeAndValidateCsdl(this EdmModel model, XmlWriter writer)
        {
            var validationErrors = new List<DataModelErrorEventArgs>();

            var serializer = new CsdlSerializer();
            serializer.OnError += (_, e) => validationErrors.Add(e);
            serializer.Serialize(model, writer);

            return validationErrors;
        }

        private static List<DataModelErrorEventArgs> SerializeAndValidateSsdl(this EdmModel model, XmlWriter writer)
        {
            var providerInfoProperty = typeof(EdmModel).GetProperty("ProviderInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            var providerInfo = (DbProviderInfo)providerInfoProperty.GetValue(model);

            var validationErrors = new List<DataModelErrorEventArgs>();

            var serializer = new SsdlSerializer();
            serializer.OnError += (_, e) => validationErrors.Add(e);
            serializer.Serialize(model, providerInfo.ProviderInvariantName, providerInfo.ProviderManifestToken, writer, true);

            return validationErrors;
        }

        private static string ToText(DataModelErrorEventArgs errorEventArgs)
        {
            return string.Format("{0}.{1}: {2}", errorEventArgs.Item, errorEventArgs.PropertyName, errorEventArgs.ErrorMessage);
        }

        public enum EdmModelType
        {
            Conceptual,
            Store,
        }
    }
}