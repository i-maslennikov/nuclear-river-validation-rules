using System;
using System.Data.Common;
using System.Transactions;

using LinqToDB.Data;

using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;
using NuClear.Storage.LinqToDB.Writings;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    public class StubReadableDomainContextProvider : IReadableDomainContextProvider
    {
        private readonly IReadableDomainContext _readableDomainContext;

        public StubReadableDomainContextProvider(DbConnection connection, DataConnection dataContext)
        {
            _readableDomainContext = CreateReadableDomainContext(connection, dataContext);
        }

        public IReadableDomainContext Get()
        {
            return _readableDomainContext;
        }

        private static IReadableDomainContext CreateReadableDomainContext(DbConnection connection, DataConnection dataContext)
        {
            return new LinqToDBDomainContext(connection,
                                             dataContext,
                                             new NullIManagedConnectionStateScopeFactory(),
                                             new WritingStrategyFactory(),
                                             new TransactionOptions
                                             {
                                                 IsolationLevel = IsolationLevel.ReadCommitted,
                                                 Timeout = TimeSpan.Zero
                                             },
                                             new NullPendingChangesHandlingStrategy());
        }
    }
}