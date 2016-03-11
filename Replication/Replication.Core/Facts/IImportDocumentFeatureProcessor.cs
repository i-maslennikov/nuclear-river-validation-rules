namespace NuClear.Replication.Core.Facts
{
    public interface IImportDocumentFeatureProcessor<TDto>
    {
        void Import(TDto statisticsDto);
    }
}