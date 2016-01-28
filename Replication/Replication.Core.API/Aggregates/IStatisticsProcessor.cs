namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IStatisticsProcessor
    {
        void RecalculateStatistics(StatisticsProcessorSlice slice);
    }
}