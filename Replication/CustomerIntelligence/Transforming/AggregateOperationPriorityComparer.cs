using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal sealed class AggregateOperationPriorityComparer : IComparer<Type>
    {
        private static readonly IReadOnlyDictionary<Type, int> Priority
            = new Dictionary<Type, int>
              {
                  { typeof(InitializeAggregate), 3 },
                  { typeof(RecalculateAggregate), 2 },
                  { typeof(DestroyAggregate), 1 },
              };

        public int Compare(Type x, Type y)
        {
            return Priority[x] - Priority[y];
        }
    }
}