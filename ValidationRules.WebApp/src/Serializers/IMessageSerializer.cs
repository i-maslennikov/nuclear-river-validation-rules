using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public interface IMessageSerializer
    {
        MessageTypeCode MessageType { get; }
        MessageTemplate Serialize(ValidationResult validationResult);
    }
}