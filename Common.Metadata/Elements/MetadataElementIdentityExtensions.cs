using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.River.Common.Metadata.Elements
{
    internal static class MetadataElementIdentityExtensions
    {
        public static string ResolveFullName(this IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.ResolvePath().Replace('/', '.');
        }

        public static string ResolveName(this IMetadataElementIdentity identity)
        {
            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.ResolvePath().Split('/').LastOrDefault();
        }

        private static string ResolvePath(this IMetadataElementIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            if (identity.Id == null)
            {
                throw new InvalidOperationException("The id was not specified.");
            }

            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        }
    }
}