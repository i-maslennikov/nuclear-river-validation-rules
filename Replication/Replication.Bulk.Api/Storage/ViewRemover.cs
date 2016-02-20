using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.API.Storage
{
    public sealed class ViewRemover
    {
        private readonly IConnectionStringSettings _connectionStringSettings;

        public ViewRemover(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public IDisposable TemporaryRemoveViews(IConnectionStringIdentity connectionStringIdentity, IEnumerable<string> essentialViewNames)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(connectionStringIdentity);
            var database = GetDatabase(connectionString);
            var views = new List<StringCollection>();
            foreach (var view in database.Views.Cast<View>()
                .Where(v => !v.IsSystemObject)
                .Where(v => !essentialViewNames.Contains($"{v.Schema}.{v.Name}", StringComparer.InvariantCultureIgnoreCase)).ToArray())
            {
                views.Add(view.Script());
                view.Drop();
            }

            return new ViewContainer(connectionString, views);
        }

        private static Database GetDatabase(string connectionString)
        {
            var connection = new ServerConnection { ConnectionString = connectionString };
            var server = new Server(connection);

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            return server.Databases[connectionStringBuilder.InitialCatalog];
        }

        private class ViewContainer : IDisposable
        {
            private readonly string _connectionString;
            private readonly List<StringCollection> _views;
            private bool _disposed;

            public ViewContainer(string connectionString, List<StringCollection> views)
            {
                _connectionString = connectionString;
                _views = views;
            }

            ~ViewContainer()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (disposing)
                {
                    using (var sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();

                        foreach (var view in _views)
                        {
                            foreach (var s in view)
                            {
                                var command = new SqlCommand(s, sqlConnection);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }
}