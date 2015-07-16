﻿using System;
using System.Data;
using System.Transactions;

using LinqToDB.Data;

using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace NuClear.AdvancedSearch.Replication.Tests
{
    public class StubReadableDomainContextProvider : IReadableDomainContextProvider
    {
        private readonly IReadableDomainContext _readableDomainContext;

        public StubReadableDomainContextProvider(IDbConnection connection, DataConnection dataContext)
        {
            _readableDomainContext = CreateReadableDomainContext(connection, dataContext);
        }

        public IReadableDomainContext Get()
        {
            return _readableDomainContext;
        }

        private static IReadableDomainContext CreateReadableDomainContext(IDbConnection connection, DataConnection dataContext)
        {
            return new LinqToDBDomainContext(connection,
                                             dataContext,
                                             new NullIManagedConnectionStateScopeFactory(),
                                             new TransactionOptions
                                             {
                                                 IsolationLevel = IsolationLevel.ReadCommitted,
                                                 Timeout = TimeSpan.Zero
                                             },
                                             new NullPendingChangesHandlingStrategy());
        }
    }
}