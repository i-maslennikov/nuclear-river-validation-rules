using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace NuClear.Querying.Storage
{
    public sealed class SqlServerConfiguration : DbConfiguration
    {
        public SqlServerConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}