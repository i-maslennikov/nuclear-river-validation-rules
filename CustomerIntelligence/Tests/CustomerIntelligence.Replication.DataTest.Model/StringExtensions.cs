using System;
using System.Data.SqlClient;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    internal static class StringExtensions
    {
        private static readonly string DatabaseNamePostfix = Guid.NewGuid().ToString("N");

        public static string ProtectSqlConnectionString(this string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = builder.InitialCatalog + DatabaseNamePostfix;
            return builder.ToString();
        }
    }
}