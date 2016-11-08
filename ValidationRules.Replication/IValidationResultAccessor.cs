using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    public interface IValidationResultAccessor : IStorageBasedDataObjectAccessor<Version.ValidationResult>
    {
        int MessageTypeId { get; }
    }
}