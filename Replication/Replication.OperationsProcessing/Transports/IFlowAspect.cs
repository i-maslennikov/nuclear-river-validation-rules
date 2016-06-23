namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IFlowAspect<TEvent>
    {
        bool ShouldEventBeLogged(TEvent @event);
        void ReportMessageLoggedCount(long count);
    }
}