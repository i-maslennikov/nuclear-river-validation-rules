using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    // stateinit only accessor
    public sealed class SystemStatusAccessor : IStorageBasedDataObjectAccessor<SystemStatus>
    {
        // ReSharper disable once UnusedParameter.Local
        public SystemStatusAccessor(IQuery query)
        {
        }

        public IQueryable<SystemStatus> GetSource() => new[]
        {
            new SystemStatus { Id = SystemStatus.SystemId.Ams, SystemIsDown = false }
        }.AsQueryable();


        public FindSpecification<SystemStatus> GetFindSpecification(IReadOnlyCollection<ICommand> commands) => throw new NotSupportedException();
    }
}
