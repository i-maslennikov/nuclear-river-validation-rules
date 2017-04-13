namespace NuClear.ValidationRules.OperationsProcessing
{
    public interface IReceiverTelemetryReporter
    {
        void Peeked(int count);
        void Completed(int count);
        void Failed(int count);
    }
}