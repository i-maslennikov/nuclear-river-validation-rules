using System.Collections.Generic;
using System.Xml.Linq;

namespace NuClear.ValidationRules.SingleCheck.FieldComparer
{
    internal class XDocumentComparer : IEqualityComparer<XDocument>
    {
        // XNodeEqualityComparer использует DeepEquals и соответствующий hashCode
        private readonly XNodeEqualityComparer _comparer = new XNodeEqualityComparer();

        public bool Equals(XDocument x, XDocument y)
        {
            return _comparer.Equals(x, y);
        }

        public int GetHashCode(XDocument obj)
        {
            return _comparer.GetHashCode(obj);
        }
    }
}