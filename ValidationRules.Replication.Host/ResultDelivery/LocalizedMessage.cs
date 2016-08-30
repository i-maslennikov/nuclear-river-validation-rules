namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class LocalizedMessage
    {
        public LocalizedMessage(Result result, string header, string message)
        {
            Result = result;
            Header = header;
            Message = message;
        }

        public Result Result { get; }
        public string Header { get; }
        public string Message { get; }
    }
}