using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.River.Common.Metadata.Elements
{
    public static class MetadataElementExtensions
    {
        public static string ResolveFullName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.Identity.ResolveFullName();
        }

        public static string ResolveName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.Identity.ResolveName();
        }
   }
}