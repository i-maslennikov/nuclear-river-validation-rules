using System.Xml.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class Message
    {
        public Message(MessageTypeCode messageType, XDocument data, int resultCode)
        {
            MessageType = messageType;
            Data = data;
            ResultCode = resultCode;
        }

        public MessageTypeCode MessageType { get; }
        public XDocument Data { get; }
        public int ResultCode { get; }
    }
}