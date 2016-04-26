using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace NuClear.StateInitialization.Core.Storage
{
    public static class ViewRemover
    {
        public static IDisposable TemporaryRemoveViews(string connectionString)
        {
            var database = GetDatabase(connectionString);
            var existingViews = database.Views.Cast<View>().Where(v => !v.IsSystemObject).ToArray();
            var viewsToRestore = new List<StringCollection>();
            foreach (var existingView in existingViews)
            {
                viewsToRestore.Add(existingView.Script());
                existingView.Drop();
            }

            return new ViewContainer(connectionString, viewsToRestore);
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