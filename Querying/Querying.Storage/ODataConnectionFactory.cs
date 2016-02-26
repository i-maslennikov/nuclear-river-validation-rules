using System;
using System.Data.Common;
using System.Data.SqlClient;

using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Querying.Storage
{
    public sealed class ODataConnectionFactory : IDbConnectionFactory
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;

        public ODataConnectionFactory(IMetadataProvider metadataProvider, IConnectionStringSettings connectionStringSettings)
        {
            _metadataProvider = metadataProvider;
            _connectionStringSettings = connectionStringSettings;
        }

        public DbConnection CreateConnection(Uri contextId)
        {
            BoundedContextElement contextElement;
            if (!_metadataProvider.TryGetMetadata(contextId, out contextElement))
            {
                return null;
            }

            var connectionString = _connectionStringSettings.GetConnectionString(CustomerIntelligenceConnectionStringIdentity.Instance);
            return new SqlConnection(connectionString);
        }
    }
}