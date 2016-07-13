using System.Xml.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        public int MessageType { get; }
        public LocalizedMessage Serialize(XDocument messageParams)
        {
            throw new System.NotImplementedException();
        }
    }
}