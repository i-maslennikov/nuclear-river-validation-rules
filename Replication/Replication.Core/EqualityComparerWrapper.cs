using System;
using System.Collections.Generic;

namespace NuClear.Replication.Core
{
    public sealed class EqualityComparerWrapper<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equality;
        private readonly Func<T, int> _hashCode;

        public EqualityComparerWrapper(Func<T, T, bool> equality, Func<T, int> hashCode)
        {
            _equality = equality;
            _hashCode = hashCode;
        }

        public bool Equals(T x, T y)
        {
            return _equality.Invoke(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _hashCode.Invoke(obj);
        }
    }
}