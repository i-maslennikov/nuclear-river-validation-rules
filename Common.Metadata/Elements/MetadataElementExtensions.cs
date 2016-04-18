using System;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.River.Common.Metadata.Elements
{
    public static class MetadataElementExtensions
    {
        public static string ResolveFullName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return element.Identity.ResolvePath().Replace('/', '.');
        }

        public static string ResolveName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return element.Identity.ResolvePath().Split('/').LastOrDefault();
        }

        private static string ResolvePath(this IMetadataElementIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            if (identity.Id == null)
            {
                throw new InvalidOperationException("The id was not specified.");
            }

            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        }
    }
}