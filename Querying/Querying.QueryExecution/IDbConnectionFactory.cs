using System;
using System.Data.Common;

namespace NuClear.Querying.Edm
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection(Uri contextId);
    }
}