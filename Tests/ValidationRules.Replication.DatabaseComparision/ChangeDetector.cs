using System;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.FieldComparer;

namespace ValidationRules.Replication.DatabaseComparision
{
    internal abstract class ChangeDetector
    {
        private static readonly EqualityComparerFactory EqualityComparerFactory =
            new EqualityComparerFactory(new LinqToDbPropertyProvider(TypeProvider.Facts, TypeProvider.Aggregates, TypeProvider.Messages),
                                        new DateTimeComparer(),
                                        new XDocumentComparer());

        public static ChangeDetector Create(Type dataObjectType)
        {
            return (ChangeDetector)Activator.CreateInstance(typeof(DefaultChangeDetector<>).MakeGenericType(dataObjectType));
        }

        public abstract void Process(string sourceDb, MappingSchema sourceSchema, string destDb, MappingSchema destSchema);

        private sealed class DefaultChangeDetector<T> : ChangeDetector
            where T : class
        {
            private readonly IDataObjectReader<T> _objectReader = (IDataObjectReader<T>)Activator.CreateInstance(ResolveReaderType());
            private readonly Saver<T> _saver = new Saver<T>();

            public override void Process(string sourceDb, MappingSchema sourceSchema, string destDb, MappingSchema destSchema)
            {
                Console.Write($"{typeof(T).FullName}... ");
                var sw = Stopwatch.StartNew();

                using (var source = new DataConnection(sourceDb).AddMappingSchema(sourceSchema))
                using (var dest = new DataConnection(destDb).AddMappingSchema(destSchema))
                {
                    var accessorTypes = TypeProvider.GetAccessors(destSchema, typeof(T));
                    var completeChanges = new EntityChanges<T>(_objectReader.ReadSource(source, accessorTypes), _objectReader.ReadDest(dest), EqualityComparerFactory.CreateCompleteComparer<T>());
                    var changes = new EntityChanges<T>(completeChanges.SourceOnly, completeChanges.DestOnly, EqualityComparerFactory.CreateIdentityComparer<T>());
                    _saver.Save(changes);
                }

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds / 1000);
            }

            private static Type ResolveReaderType()
                => typeof(ChangeDetector).Assembly.GetTypes().FirstOrDefault(x => x.IsClass && !x.IsAbstract && typeof(IDataObjectReader<T>).IsAssignableFrom(x)) ??
                   typeof(DefaultDataObjectReader<T>);
        }
    }
}
