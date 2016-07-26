namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public interface IMessageSerializer
    {
        int MessageType { get; }
        LocalizedMessage Serialize(Message message);
    }
}