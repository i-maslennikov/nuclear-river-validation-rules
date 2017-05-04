using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Storage.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace ValidationRules.Replication.DatabaseComparision
{
    internal interface IDataObjectReader<T>
    {
        IEnumerable<T> ReadDest(DataConnection db);
        IEnumerable<T> ReadSource(DataConnection db, IEnumerable<Type> accessorTypes);
    }

    internal sealed class DefaultDataObjectReader<T> : IDataObjectReader<T>
        where T : class
    {
        public IEnumerable<T> ReadDest(DataConnection db)
            => db.GetTable<T>();

        public IEnumerable<T> ReadSource(DataConnection db, IEnumerable<Type> accessorTypes)
            => accessorTypes.Select(x => Activator.CreateInstance(x, new LinqToDbQuery(db)))
                            .Cast<IStorageBasedDataObjectAccessor<T>>()
                            .SelectMany(x => x.GetSource());
    }

    internal sealed class ValidationResultDataObjectReader : IDataObjectReader<Version.ValidationResult>
    {
        public IEnumerable<Version.ValidationResult> ReadDest(DataConnection db)
            => db.GetTable<Version.ValidationResult>().ForVersion(db.GetTable<Version>().Max(x => x.Id)).AsEnumerable().Select(ClearVersionId);

        public IEnumerable<Version.ValidationResult> ReadSource(DataConnection db, IEnumerable<Type> accessorTypes)
            => accessorTypes.Select(x => Activator.CreateInstance(x, new LinqToDbQuery(db)))
                            .Cast<IStorageBasedDataObjectAccessor<Version.ValidationResult>>()
                            .SelectMany(x => x.GetSource());

        internal static Version.ValidationResult ClearVersionId(Version.ValidationResult result)
            => new Version.ValidationResult
                {
                    MessageType = result.MessageType,
                    MessageParams = result.MessageParams,
                    PeriodEnd = result.PeriodEnd,
                    PeriodStart = result.PeriodStart,
                    ProjectId = result.ProjectId,
                    OrderId = result.OrderId,
                    Resolved = result.Resolved,
                };
    }

}
