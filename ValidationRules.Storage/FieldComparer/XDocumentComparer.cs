using System.Collections.Generic;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.FieldComparer
{
    public sealed class XDocumentComparer : IEqualityComparer<XDocument>
    {
        // XNodeEqualityComparer использует DeepEquals и соответствующий hashCode
        private static readonly XNodeEqualityComparer Comparer = XNode.EqualityComparer;

        public bool Equals(XDocument x, XDocument y)
        {
            return Comparer.Equals(x, y);
        }

        public int GetHashCode(XDocument obj)
        {
            return Comparer.GetHashCode(obj);
        }
    }
}