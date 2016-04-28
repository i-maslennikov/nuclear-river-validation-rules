using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Data;

using Moq;

using NuClear.CustomerIntelligence.Replication.Tests.MemoryDb;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    internal abstract class DataFixtureBase : FixtureBase
    {
        static DataFixtureBase()
        {
#if DEBUG
            //LinqToDB.Common.Configuration.Linq.GenerateExpressionTest = true;
            DataConnection.TurnTraceSwitchOn(TraceLevel.Verbose);
            DataConnection.WriteTraceLine = (s1, s2) => Debug.WriteLine(s1, s2);
#endif
        }

        private StubDomainContextProvider _stubDomainContextProvider;
        private MockLinqToDbDataBuilder _mockLinqToDbDataBuilder;

        [SetUp]
        public void FixtureBuildUp()
        {
            _stubDomainContextProvider = new StubDomainContextProvider();
            _mockLinqToDbDataBuilder = new MockLinqToDbDataBuilder(_stubDomainContextProvider);
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _stubDomainContextProvider.Dispose();
        }

        protected IQuery Query => new Query(_stubDomainContextProvider);

        protected MockLinqToDbDataBuilder SourceDb => _mockLinqToDbDataBuilder;

        protected MockLinqToDbDataBuilder TargetDb => _mockLinqToDbDataBuilder;

        protected IRepositoryFactory RepositoryFactory => new LinqToDBRepositoryFactory(_stubDomainContextProvider);

        protected IEnumerable<long> In(params long[] ids)
        {
            return ids;
        }

        protected interface IRepositoryFactory
        {
			IBulkRepository<T> Create<T>() where T : class;

	        object Create(Type type);
        }

        protected class LinqToDBRepositoryFactory : IRepositoryFactory
        {
            private readonly StubDomainContextProvider _stubDomainContextProvider;

            public LinqToDBRepositoryFactory(StubDomainContextProvider stubDomainContextProvider)
            {
                _stubDomainContextProvider = stubDomainContextProvider;
            }

            public IBulkRepository<T> Create<T>()
                where T : class
            {
                return new BulkRepository<T>(new LinqToDBRepository<T>(_stubDomainContextProvider));
            }

			public object Create(Type type)
			{
				var repository = Activator.CreateInstance(typeof(LinqToDBRepository<>).MakeGenericType(type), _stubDomainContextProvider);
				return Activator.CreateInstance(typeof(BulkRepository<>).MakeGenericType(type), repository);
			}
		}

	    protected class VerifiableRepositoryFactory : IRepositoryFactory
	    {
		    private readonly IDictionary<Type, IRepository> _cache;

		    public VerifiableRepositoryFactory()
		    {
			    _cache = new Dictionary<Type, IRepository>();
		    }

		    public IBulkRepository<T> Create<T>()
			    where T : class
		    {
			    return new BulkRepositoryStub<T>((IRepository<T>)ResolveRepository(typeof(T)));
		    }

		    public object Create(Type type)
		    {
			    var bulkRepoType = typeof(BulkRepositoryStub<>).MakeGenericType(type);
			    return Activator.CreateInstance(bulkRepoType, ResolveRepository(type));
		    }

		    public void Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times, string failMessage = null)
			    where T : class
		    {
			    var repository = (IRepository<T>)ResolveRepository(typeof(T));
			    var mock = Mock.Get(repository);
			    mock.Verify(expression, times, failMessage);
		    }

		    private IRepository ResolveRepository(Type entityType)
		    {
				IRepository repository;
			    if (!_cache.TryGetValue(entityType, out repository))
			    {
					var repoType = typeof(IRepository<>).MakeGenericType(entityType);
					var mockType = typeof(Mock<>).MakeGenericType(repoType);
					var mock = (Mock)Activator.CreateInstance(mockType);
					_cache[entityType] = repository = (IRepository)mock.Object;
				}

				return repository;
			}

	        private class BulkRepositoryStub<T> : IBulkRepository<T> where T : class
	        {
		        private readonly IRepository<T> _repository;

		        public BulkRepositoryStub(IRepository<T> repository)
		        {
			        _repository = repository;
		        }

		        public void Create(IEnumerable<T> objects)
		        {
			        Foreach(objects, _repository.Add);
		        }

		        public void Update(IEnumerable<T> objects)
		        {
					Foreach(objects, _repository.Update);
				}

		        public void Delete(IEnumerable<T> objects)
		        {
					Foreach(objects, _repository.Delete);
				}

		        private static void Foreach(IEnumerable<T> objects, Action<T> action)
		        {
			        foreach (var obj in objects)
			        {
				        action.Invoke(obj);
			        }
		        }
	        }
        }
    }
}