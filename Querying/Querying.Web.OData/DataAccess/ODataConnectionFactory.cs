using System;
using System.Data.Common;
using System.Data.SqlClient;

using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.Metamodeling.Provider;
using NuClear.River.Common.Metadata.Elements;

using IConnectionStringSettings = NuClear.Storage.API.ConnectionStrings.IConnectionStringSettings;

namespace NuClear.Querying.Web.OData.DataAccess
{
    public sealed class ODataConnectionFactory
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