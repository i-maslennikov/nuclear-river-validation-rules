using System;

namespace NuClear.Querying.Edm
{
    public sealed class BoundedContextIdentityAnnotation
    {
        public BoundedContextIdentityAnnotation(Uri identity)
        {
            Identity = identity;
        }

        public Uri Identity { get; set; }
    }
}