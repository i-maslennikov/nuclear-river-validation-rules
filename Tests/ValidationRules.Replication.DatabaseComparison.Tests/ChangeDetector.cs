using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using LinqToDB.Data;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.FieldComparer;

namespace ValidationRules.Replication.DatabaseComparison.Tests
{
    internal abstract class ChangeDetector
    {
        public static TimeSpan WaitBetweenProcess = TimeSpan.FromSeconds(60);

        private static readonly EqualityComparerFactory EqualityComparerFactory =
            new EqualityComparerFactory(new LinqToDbPropertyProvider(
                                            StorageDescriptor.Facts.MappingSchema,
                                            StorageDescriptor.Aggregates.MappingSchema,
                                            StorageDescriptor.Messages.MappingSchema),
                                        new DateTimeComparer(),
                                        new XDocumentComparer());

        public static ChangeDetector Create(Type dataObjectType)
        {
            return (ChangeDetector)Activator.CreateInstance(typeof(DefaultChangeDetector<>).MakeGenericType(dataObjectType));
        }

        public abstract EntityChangesDto ProcessTwice(StorageDescriptor sourceDescriptor, StorageDescriptor destDescriptor);

        private sealed class DefaultChangeDetector<T> : ChangeDetector
            where T : class
        {
            private readonly IDataObjectReader<T> _objectReader = (IDataObjectReader<T>)Activator.CreateInstance(ResolveReaderType());
            private static readonly IEqualityComparer<T> IdentityComparer = EqualityComparerFactory.CreateIdentityComparer<T>();
            private static readonly IEqualityComparer<T> CompleteComparer = EqualityComparerFactory.CreateCompleteComparer<T>();

            public override EntityChangesDto ProcessTwice(StorageDescriptor sourceDescriptor, StorageDescriptor destDescriptor)
            {
                var changes1 = Process(sourceDescriptor, destDescriptor);
                Thread.Sleep(WaitBetweenProcess);
                var changes2 = Process(sourceDescriptor, destDescriptor);

                // SourceChanged => Intersect
                return new EntityChangesDto
                {
                    SourceOnly = new EntityChanges<T>(changes1.SourceOnly, changes2.SourceOnly, IdentityComparer).SourceChanged,
                    DestOnly = new EntityChanges<T>(changes1.DestOnly, changes2.DestOnly, IdentityComparer).SourceChanged,
                    SourceChanged = new EntityChanges<T>(changes1.SourceChanged, changes2.SourceChanged, IdentityComparer).SourceChanged,
                    DestChanged = new EntityChanges<T>(changes1.DestChanged, changes2.DestChanged, IdentityComparer).SourceChanged
                };
            }

            private EntityChanges<T> Process(StorageDescriptor sourceDescriptor, StorageDescriptor destDescriptor)
            {
                using (var source = new DataConnection(sourceDescriptor.ConnectionStringName).AddMappingSchema(sourceDescriptor.MappingSchema))
                using (var dest = new DataConnection(destDescriptor.ConnectionStringName).AddMappingSchema(destDescriptor.MappingSchema))
                {
                    var accessorTypes = TypeProvider.GetAccessorTypes(destDescriptor.MappingSchema, typeof(T));
                    var completeChanges = new EntityChanges<T>(_objectReader.ReadSource(source, accessorTypes), _objectReader.ReadDest(dest), CompleteComparer);
                    var changes = new EntityChanges<T>(completeChanges.SourceOnly, completeChanges.DestOnly, IdentityComparer);
                    return changes;
                }
            }

            private static Type ResolveReaderType()
                => typeof(ChangeDetector).Assembly.GetTypes().FirstOrDefault(x => x.IsClass && !x.IsAbstract && typeof(IDataObjectReader<T>).IsAssignableFrom(x)) ??
                   typeof(DefaultDataObjectReader<T>);
        }
    }
}
