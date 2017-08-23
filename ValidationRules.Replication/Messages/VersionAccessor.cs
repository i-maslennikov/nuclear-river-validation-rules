using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    /// <summary>
    /// Предназначен для того, чтобы StateInitialization его нашла и создала нулевую версию.
    /// </summary>
    public sealed class VersionAccessor : IStorageBasedDataObjectAccessor<Version>
    {
        public VersionAccessor(IQuery query)
        {
        }

        public IQueryable<Version> GetSource()
            => new[] { new Version { Id = 0, UtcDateTime = DateTime.UtcNow } }.AsQueryable();

        public FindSpecification<Version> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class AmsStateAccessor : IMemoryBasedDataObjectAccessor<Version.AmsState>
    {
        public AmsStateAccessor(IQuery query)
        {
        }

        public IReadOnlyCollection<Version.AmsState> GetDataObjects(ICommand command)
        {
            throw new NotImplementedException();
        }

        public FindSpecification<Version.AmsState> GetFindSpecification(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}
