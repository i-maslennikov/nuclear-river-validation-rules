using System.Collections.Generic;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public interface IDeserializer<in TMessage, out TDto>
    {
        IReadOnlyCollection<TDto> Deserialize(TMessage message);
    }
}