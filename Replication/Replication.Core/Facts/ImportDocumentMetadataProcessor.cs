using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.Core.Facts
{
    public class ImportDocumentMetadataProcessor<TDto> : IImportDocumentMetadataProcessor
        where TDto : class
    {
        private readonly ImportDocumentMetadata<TDto> _metadata;
        private readonly IReadOnlyCollection<IImportDocumentFeatureProcessor<TDto>> _featureProcessors;

        public ImportDocumentMetadataProcessor(ImportDocumentMetadata<TDto> metadata, IReadOnlyCollection<IImportDocumentFeatureProcessor<TDto>> featureProcessors)
        {
            _metadata = metadata;
            _featureProcessors = featureProcessors;
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
                foreach (var importer in _featureProcessors)
                {
                    importer.Import(statisticsDto);
                }

                transaction.Complete();
            }

            return _metadata.RecalculationSpecification.Map(statisticsDto);
        }
    }
}