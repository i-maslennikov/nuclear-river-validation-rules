using System;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IImportDocumentMetadataProcessorFactory
    {
        IImportDocumentMetadataProcessor Create(Type statisticsDtoType);
    }
}