using System;
using System.Collections.Generic;


namespace NuClear.ValidationRules.Domain.Model
{
    public sealed class FactTypePriorityComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            // Контроля целостности на уровне БД нет, поэтому порядок безразличен
            return x.GetHashCode() - y.GetHashCode();
        }
    }
}