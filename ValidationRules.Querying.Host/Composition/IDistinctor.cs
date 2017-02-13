using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public interface IDistinctor
    {
        MessageTypeCode MessageType { get; }
        IEnumerable<Message> Distinct(IEnumerable<Message> messages);
    }
}