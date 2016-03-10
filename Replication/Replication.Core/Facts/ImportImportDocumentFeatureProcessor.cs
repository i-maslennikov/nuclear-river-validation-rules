using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Facts
{
    public sealed class ImportDocumentFeatureProcessor<TDto, TFact> : IImportDocumentFeatureProcessor<TDto>
        where TFact : class
    {
        private readonly ImportDocumentFeature<TDto, TFact> _feature;
        private readonly IQuery _query;
        private readonly IBulkRepository<TFact> _repository;

        public ImportDocumentFeatureProcessor(ImportDocumentFeature<TDto, TFact> feature, IQuery query, IBulkRepository<TFact> repository)
        {
            _feature = feature;
            _query = query;
            _repository = repository;
        }

        public void Import(TDto statisticsDto)
        {
            _repository.Delete(_query.For(_feature.FindSpecificationProvider.Invoke(statisticsDto)));
            _repository.Create(_feature.MapSpecification.Map(statisticsDto));
        }
    }
}