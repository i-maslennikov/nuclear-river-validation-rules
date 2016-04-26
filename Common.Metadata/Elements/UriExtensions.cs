using System;

namespace NuClear.Querying.Metadata.Elements
{
    internal static class UriExtensions
    {
        public static Uri AsUri(this string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }
    }
}