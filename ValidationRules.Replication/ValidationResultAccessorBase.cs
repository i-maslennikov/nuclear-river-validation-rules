using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    public abstract class ValidationResultAccessorBase : IValidationResultAccessor
    {
        private readonly IQuery _query;
        private readonly int _messageTypeId;

        protected ValidationResultAccessorBase(IQuery query, MessageTypeCode messageTypeId)
        {
            _query = query;
            _messageTypeId = (int)messageTypeId;
        }

        int IValidationResultAccessor.MessageTypeId => _messageTypeId;

        public IQueryable<Version.ValidationResult> GetSource()
            => GetValidationResults(_query).ApplyMessageType(_messageTypeId);

        public FindSpecification<Version.ValidationResult> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<Version.ValidationResult>(x => x.MessageType == _messageTypeId);

        protected abstract IQueryable<Version.ValidationResult> GetValidationResults(IQuery query);
    }
}