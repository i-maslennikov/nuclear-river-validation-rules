using System;
using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class AggregateCommandPriorityComparer : IComparer<Type>
    {
        private static readonly IReadOnlyDictionary<Type, int> Priority
            = new Dictionary<Type, int>
              {
                  { typeof(DestroyAggregateCommand), 3 },
                  { typeof(InitializeAggregateCommand), 2 },
                  { typeof(RecalculateAggregateCommand), 1 },
              };
        public int Compare(Type x, Type y)
        {
            return Priority[x] - Priority[y];
        }
    }
}