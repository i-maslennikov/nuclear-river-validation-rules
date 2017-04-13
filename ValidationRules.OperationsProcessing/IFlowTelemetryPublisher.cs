namespace NuClear.ValidationRules.OperationsProcessing
{
    public interface IFlowTelemetryPublisher
    {
        void Peeked(int count);
        void Completed(int count);
        void Failed(int count);
    }
}