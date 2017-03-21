using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    /// <summary>
    /// Предназначен для того, чтобы StateInitialization его нашла и наполнила таблицу ValidationResultType
    /// </summary>
    public sealed class ValidationResultTypeAccessor : IStorageBasedDataObjectAccessor<Version.ValidationResultType>
    {
        public ValidationResultTypeAccessor(IQuery query)
        {
        }

        public IQueryable<Version.ValidationResultType> GetSource()
        {
            var resultTypes = ResultTypeMap.Map.SelectMany(x => x.Value.Select(y => new Version.ValidationResultType
            {
                ResultType = x.Key,
                MessageType = (int)y.Key,
                Result = y.Value,
            }));

            return resultTypes.AsQueryable();
        }

        public FindSpecification<Version.ValidationResultType> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            throw new NotSupportedException();
        }
    }
}
