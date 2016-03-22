using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.River.Common.Metadata.Tests
{
    internal static class MetadataExtensions
    {
        private static readonly HashSet<string> MainProperties = new HashSet<string>(new[] { "Identity", "Kind", "Elements", "Features" });

        [Conditional("DEBUG")]
        public static void Dump(this IMetadataElement element, MetadataKind metadata, Type[] ignoredFeatures = null)
        {
            Debug.WriteLine(element.ToJson(true, ResolvePropertyNames(metadata), ignoredFeatures));
        }

        [Conditional("DEBUG")]
        public static void Dump(this IMetadataElement element, string[] properties, Type[] ignoredFeatures = null)
        {
            Debug.WriteLine(element.ToJson(true, new HashSet<string>(properties), ignoredFeatures));
        }

        public static string Serialize(this IMetadataElement element, MetadataKind metadata, Type[] ignoredFeatures = null)
        {
            return element.ToJson(false, ResolvePropertyNames(metadata), ignoredFeatures).Replace("\"", "'");
        }

        public static string Serialize(this IMetadataElement element, string[] properties, Type[] ignoredFeatures = null)
        {
            return element.ToJson(false, new HashSet<string>(properties), ignoredFeatures).Replace("\"", "'");
        }

        private static string ToJson(this IMetadataElement value, bool indented, HashSet<string> properties, Type[] ignoredFeatures = null)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ElementContractResolver(properties, ignoredFeatures),
                Converters = new JsonConverter[] { new StringEnumConverter() }
            };
            return JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None, settings);
        }

        private static HashSet<string> ResolvePropertyNames(MetadataKind metadata)
        {
            var propertyNames = metadata.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            return propertyNames.Length > 0 ? new HashSet<string>(MainProperties.Intersect(propertyNames)) : MainProperties;
        }

        private class ElementContractResolver : DefaultContractResolver
        {
            private readonly HashSet<string> _properties;
            private readonly Type[] _ignoredFeatures;

            public ElementContractResolver(HashSet<string> propertyNames, Type[] ignoredFeatures)
            {
                _properties = propertyNames;
                _ignoredFeatures = ignoredFeatures ?? new Type[0];
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (typeof(IMetadataElement).IsAssignableFrom(property.DeclaringType))
                {
                    property.ShouldSerialize = _ => _properties.Contains(property.PropertyName);
                    if (property.PropertyName.Equals("Features") && _ignoredFeatures.Any())
                    {
                        property.ValueProvider = new Provider(property.ValueProvider, _ignoredFeatures);
                    }
                }

                return property;
            }

            private class Provider : IValueProvider
            {
                private readonly IValueProvider _valueProvider;
                private readonly HashSet<Type> _ignoredFeatures;

                public Provider(IValueProvider valueProvider, Type[] ignoredFeatures)
                {
                    _valueProvider = valueProvider;
                    _ignoredFeatures = new HashSet<Type>(ignoredFeatures);
                }

                public object GetValue(object target)
                {
                    var value = _valueProvider.GetValue(target) as IEnumerable<IMetadataFeature>;
                    if (value == null) return null;

                    return value.Where(x => !_ignoredFeatures.Contains(x.GetType()));
                }

                public void SetValue(object target, object value)
                {
                    _valueProvider.SetValue(target, value);
                }
            }
        }
    }
}