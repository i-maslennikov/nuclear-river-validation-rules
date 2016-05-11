using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Transactions;

using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Expressions;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;
using NuClear.Storage.LinqToDB.Writings;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    public class SqliteDomainContextFactory : IReadableDomainContextFactory, IModifiableDomainContextFactory, IStorageMappingDescriptorProvider, IDisposable
    {
        private const int DefaultQueryExecutionTimeout = 60;

        private static class ConnectionStringNames
        {
            public const string Erm = "Erm";
            public const string Facts = "Facts";
            public const string CustomerIntelligence = "CustomerIntelligence";
        }

        private static readonly MethodInfo CreateTableMethodInfo =
            MemberHelper.MethodOf(() => default(IDataContext).CreateTable<object>(default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  default(string),
                                                                                  DefaulNullable.None))
                        .GetGenericMethodDefinition();

        private static readonly Lazy<Type[]> Tables = new Lazy<Type[]>(
            () =>
            {
                var accessor = typeof(Schema);
                return accessor.Assembly.GetTypes()
                               .Where(t => t.IsClass && !t.Namespace.IsNullOrEmpty() && t.Namespace.Contains("Model"))
                               .ToArray();
            });

        private static readonly ConcurrentDictionary<IConnectionStringIdentity, Tuple<SQLiteConnection, DataConnection>> Connections =
            new ConcurrentDictionary<IConnectionStringIdentity, Tuple<SQLiteConnection, DataConnection>>();

        private readonly TransactionOptions _transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.Zero
        };
        private readonly IStorageMappingDescriptorProvider _storageMappingDescriptorProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly ILinqToDbModelFactory _linqToDbModelFactory;


        public SqliteDomainContextFactory(IReadOnlyDictionary<string, MappingSchema> schemaMap)
        {
            _storageMappingDescriptorProvider = new StorageMappingDescriptorProvider(new SqliteEntityContainerNameResolver(), CreateConnectionStringIdentityResolver());
            _connectionStringSettings = CreateConnectionStringSettings();
            _linqToDbModelFactory = new LinqToDbModelFactory(schemaMap, _transactionOptions, DefaultQueryExecutionTimeout);
        }

        public IReadableDomainContext Create(StorageMappingDescriptor storageMappingDescriptor)
        {
            return CreateDomainContext(storageMappingDescriptor);
        }

        public IModifiableDomainContext Create<T>() where T : class
        {
            return CreateDomainContext(GetWriteStorageMapping(typeof(T)));
        }

        public StorageMappingDescriptor GetReadStorageMapping(Type objType)
        {
            return _storageMappingDescriptorProvider.GetReadStorageMapping(objType);
        }

        public StorageMappingDescriptor GetWriteStorageMapping(Type objType)
        {
            return _storageMappingDescriptorProvider.GetWriteStorageMapping(objType);
        }

        public void Dispose()
        {
            foreach (var connection in Connections.Values)
            {
                connection.Item1.Dispose();
            }

            Connections.Clear();
        }

        private static ConnectionStringIdentityResolver CreateConnectionStringIdentityResolver()
        {
            var readConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>
                {
                    { ConnectionStringNames.Erm, ErmConnectionStringIdentity.Instance },
                    { ConnectionStringNames.Facts, FactsConnectionStringIdentity.Instance },
                    { ConnectionStringNames.CustomerIntelligence, CustomerIntelligenceConnectionStringIdentity.Instance }
                };

            var writeConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>
                {
                    { ConnectionStringNames.Erm, ErmConnectionStringIdentity.Instance },
                    { ConnectionStringNames.Facts, FactsConnectionStringIdentity.Instance },
                    { ConnectionStringNames.CustomerIntelligence, CustomerIntelligenceConnectionStringIdentity.Instance }
                };

            return new ConnectionStringIdentityResolver(readConnectionStringNameMap, writeConnectionStringNameMap);
        }

        private static IConnectionStringSettings CreateConnectionStringSettings()
        {
            return new ConnectionStringSettingsAspect(
                new Dictionary<IConnectionStringIdentity, string>
                {
                    {
                        ErmConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings[ConnectionStringNames.Erm].ConnectionString
                    },
                    {
                        FactsConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings[ConnectionStringNames.Facts].ConnectionString
                    },
                    {
                        CustomerIntelligenceConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings[ConnectionStringNames.CustomerIntelligence].ConnectionString
                    }
                });
        }

        private LinqToDBDomainContext CreateDomainContext(StorageMappingDescriptor storageMappingDescriptor)
        {
            var linqToDbModel = _linqToDbModelFactory.Create(storageMappingDescriptor.EntityContainerName);

            var connections = Connections.GetOrAdd(
                storageMappingDescriptor.ConnectionStringIdentity,
                connectionStringIdentity =>
                {
                    var dataProvider = new SQLiteDataProvider();
                    var connectionString = _connectionStringSettings.GetConnectionString(connectionStringIdentity);
                    var connection = (SQLiteConnection)dataProvider.CreateConnection(connectionString);
                    connection.Open();

                    var dataConnection = new DataConnection(dataProvider, connection)
                    {
                        CommandTimeout = linqToDbModel.QueryExecutionTimeout,
                        IsMarsEnabled = false
                    };
                    dataConnection.AddMappingSchema(linqToDbModel.MappingSchema);

                    return Tuple.Create(connection, TuneConnection(dataConnection));
                });

            return new LinqToDBDomainContext(connections.Item1,
                                             connections.Item2,
                                             new ManagedConnectionStateScopeFactory(),
                                             new WritingStrategyFactory(), 
                                             linqToDbModel.TransactionOptions,
                                             new NullPendingChangesHandlingStrategy());
        }


        private static DataConnection TuneConnection(DataConnection db)
        {
            if (db.DataProvider.Name == ProviderName.SQLite)
            {
                using (new NoSqlTrace())
                {
                    var schema = db.MappingSchema;
                    foreach (var table in Tables.Value)
                    {
                        var attributes = schema.GetAttributes<TableAttribute>(table);
                        if (attributes != null && attributes.Length > 0)
                        {
                            // SQLite does not support schemas, so prepend with it table names
                            Array.ForEach(attributes,
                                          attr =>
                                          {
                                              attr.Name = string.IsNullOrEmpty(attr.Name)
                                                              ? attr.Schema + "_" + table.Name
                                                              : attr.Schema + "_" + attr.Name;
                                              attr.Schema = null;
                                          });

                            try
                            {
                                // create empty table
                                CreateTableMethodInfo.MakeGenericMethod(table).Invoke(null, new object[] { db, null, null, null, null, null, DefaulNullable.None });
                            }
                            catch (Exception)
                            {
                                // table can be already created by previous type mapped to the same table
                                // ignore exception and continue
                            }
                        }
                    }
                }
            }

            return db;
        }
    }
}