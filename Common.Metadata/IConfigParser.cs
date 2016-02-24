using System.IO;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata
{
    public interface IConfigParser
    {
        IDataTransferObject Parse(Stream config);
    }
}
