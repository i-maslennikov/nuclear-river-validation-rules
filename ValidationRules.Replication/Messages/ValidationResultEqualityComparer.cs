using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    internal sealed class ValidationResultEqualityComparer : IEqualityComparer<Version.ValidationResult>
    {
        public static IEqualityComparer<Version.ValidationResult> Instance = new ValidationResultEqualityComparer();

        private ValidationResultEqualityComparer() { }

        public bool Equals(Version.ValidationResult x, Version.ValidationResult y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.MessageType == y.MessageType &&
                   XNode.EqualityComparer.Equals(x.MessageParams, y.MessageParams) &&
                   Date(x.PeriodStart).Equals(Date(y.PeriodStart)) &&
                   Date(x.PeriodEnd).Equals(Date(y.PeriodEnd)) &&
                   x.ProjectId == y.ProjectId &&
                   x.Result == y.Result;
        }

        public int GetHashCode(Version.ValidationResult obj)
        {
            unchecked
            {
                var hashCode = obj.MessageType;
                hashCode = (hashCode * 397) ^ XNode.EqualityComparer.GetHashCode(obj.MessageParams);
                hashCode = (hashCode * 397) ^ Date(obj.PeriodStart).GetHashCode();
                hashCode = (hashCode * 397) ^ Date(obj.PeriodEnd).GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ProjectId.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Result;
                return hashCode;
            }
        }

        // TODO: datetime2(2) не соответствует System.DateTime, поэтому загрубляем сравнение
        // в будущем лучше хранить в базе date, тогда сравнивать можно будет как есть
        private static DateTime Date(DateTime x) => x.Date;
    }
}