namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public interface IMessageSerializer
    {
        MessageTypeCode MessageType { get; }
        LocalizedMessage Serialize(Message message);
    }
}