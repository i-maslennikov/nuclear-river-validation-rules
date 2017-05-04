using System;

using LinqToDB.Mapping;

namespace ValidationRules.Replication.DatabaseComparision
{
    public static class Program
    {
        private static readonly Tuple<string, MappingSchema, string, MappingSchema>[] Stages =
            {
                Tuple.Create(nameof(TypeProvider.Erm), TypeProvider.Erm, nameof(TypeProvider.Facts), TypeProvider.Facts),
                Tuple.Create(nameof(TypeProvider.Facts), TypeProvider.Facts, nameof(TypeProvider.Aggregates), TypeProvider.Aggregates),
                Tuple.Create(nameof(TypeProvider.Aggregates), TypeProvider.Aggregates, nameof(TypeProvider.Messages), TypeProvider.Messages),
            };

        public static void Main(string[] args)
        {
            foreach (var stage in Stages)
            {
                foreach (var dataObjectType in TypeProvider.GetDataObjectTypes(stage.Item4))
                {
                    var detector = ChangeDetector.Create(dataObjectType);
                    detector.Process(stage.Item1, stage.Item2, stage.Item3, stage.Item4);
                }
            }
        }
    }
}
