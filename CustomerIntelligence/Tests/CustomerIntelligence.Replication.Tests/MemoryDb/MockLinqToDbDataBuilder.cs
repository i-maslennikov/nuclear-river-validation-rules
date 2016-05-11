using System.Linq;

using NuClear.CustomerIntelligence.Replication.Tests.Data;
using NuClear.Storage.LinqToDB;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    public sealed class MockLinqToDbDataBuilder
    {
        private readonly StubDomainContextProvider _stubDomainContextProvider;

        public MockLinqToDbDataBuilder(StubDomainContextProvider stubDomainContextProvider)
        {
            _stubDomainContextProvider = stubDomainContextProvider;
        }

        public MockLinqToDbDataBuilder Has<T>(T obj, params T[] moreObjects) where T : class
        {
            using (new NoSqlTrace())
            {
                var repository = new LinqToDBRepository<T>(_stubDomainContextProvider);
                repository.Add(obj);
                if (moreObjects != null && moreObjects.Any())
                {
                    repository.AddRange(moreObjects);
                }

                repository.Save();


                return this;
            }
        }
    }
}