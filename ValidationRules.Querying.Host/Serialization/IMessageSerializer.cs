using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization
{
    public interface IMessageSerializer
    {
        MessageTypeCode MessageType { get; }
        MessageSerializerResult Serialize(Version.ValidationResult validationResult);
    }

    public class MessageSerializerResult
    {
        public MessageSerializerResult(EntityReference mainReference, string template, params EntityReference[] references)
        {
            MainReference = mainReference;
            Template = template;
            References = references;
        }

        public EntityReference MainReference { get; set; }
        public string Template { get; set; }
        public EntityReference[] References { get; set; }
    }
}