using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Facts
{
    public class StatisticsFactImporter<TFact, TDto> : IStatisticsImporter
        where TFact : class
        where TDto : class
    {
        private readonly ImportStatisticsMetadata<TFact, TDto> _importStatisticsMetadata;
        private readonly IQuery _query;
        private readonly IBulkRepository<TFact> _repository;

        public StatisticsFactImporter(ImportStatisticsMetadata<TFact, TDto> importStatisticsMetadata, IQuery query, IBulkRepository<TFact> repository)
        {
            _importStatisticsMetadata = importStatisticsMetadata;
            _query = query;
            _repository = repository;
        }

        public IReadOnlyCollection<IOperation> Import(IDataTransferObject dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var typedDto = dto as TDto;
            if (typedDto == null)
            {
                throw new ArgumentException($"Expected dto of type {typeof(TDto).Name} but got {dto.GetType().Name}", nameof(dto));
            }

            return Import(typedDto);
        }

        private IReadOnlyCollection<IOperation> Import(TDto statisticsDto)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _repository.Delete(_query.For(_importStatisticsMetadata.FindSpecificationProvider.Invoke(statisticsDto)));
                _repository.Create(_importStatisticsMetadata.MapSpecification.Map(statisticsDto));

                transaction.Complete();
            }

            return _importStatisticsMetadata.RecalculationSpecification.Map(statisticsDto);
        }
    }
}