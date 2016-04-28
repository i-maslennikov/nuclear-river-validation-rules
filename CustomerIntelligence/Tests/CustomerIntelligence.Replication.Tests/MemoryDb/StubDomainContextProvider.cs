using System;
using System.Collections.Generic;

using LinqToDB.Mapping;

using NuClear.CustomerIntelligence.Storage;
using NuClear.Storage.Core;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    public class StubDomainContextProvider : IReadableDomainContextProvider, IModifiableDomainContextProvider, IDisposable
    {
        private readonly SqliteDomainContextFactory _sqliteDomainContextFactory;
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IDomainContextScope _domainContextScope;

        public StubDomainContextProvider()
        {
            _sqliteDomainContextFactory = new SqliteDomainContextFactory(new Dictionary<string, MappingSchema>
                                                                            {
                                                                                { "Erm", Schema.Erm },
                                                                                { "Facts", Schema.Facts },
                                                                                { "CustomerIntelligence", Schema.CustomerIntelligence }
                                                                            });
            _scopedDomainContextsStore = new ScopedDomainContextsStore(
                new CachingReadableDomainContext(_sqliteDomainContextFactory, _sqliteDomainContextFactory),
                _sqliteDomainContextFactory);
            _domainContextScope = new DomainContextScope(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
        }

        public IReadableDomainContext Get()
        {
            return _scopedDomainContextsStore.GetReadable(_domainContextScope);
        }

        public IModifiableDomainContext Get<T>() where T : class
        {
            return _scopedDomainContextsStore.GetModifiable<T>(_domainContextScope);
        }


        public void Dispose()
        {
            _sqliteDomainContextFactory.Dispose();
            _scopedDomainContextsStore.Dispose();
        }
    }
}