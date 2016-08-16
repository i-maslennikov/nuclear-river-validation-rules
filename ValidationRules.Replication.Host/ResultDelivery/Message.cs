using System.Xml.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class Message
    {
        public Message(int messageType, XDocument data, int resultCode)
        {
            MessageType = messageType;
            Data = data;
            ResultCode = resultCode;
        }

        public int MessageType { get; }
        public XDocument Data { get; }
        public int ResultCode { get; }
    }
}