using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    internal sealed class ProjectionEqualityComparer<T, TProjection> : IEqualityComparer<T>
    {
        private readonly Func<T, TProjection> _projector;

        public ProjectionEqualityComparer(Func<T, TProjection> projector)
        {
            if (projector == null)
            {
                throw new ArgumentNullException("projector");
            }

            _projector = projector;
        }

        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            var strX = ToString(x);
            var strY = ToString(y);

            return string.CompareOrdinal(strX, strY) == 0;
        }

        public int GetHashCode(T obj)
        {
            if (obj.Equals(default(T)))
            {
                throw new ArgumentNullException("obj");
            }

            return obj.GetHashCode();
        }

        private static string Serialize(TProjection obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        private string ToString(T value)
        {
            return Serialize(_projector(value));
        }
    }
}