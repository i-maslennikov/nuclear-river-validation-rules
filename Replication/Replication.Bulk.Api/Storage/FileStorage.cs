using System;
using System.IO;
using System.Linq;

using LinqToDB.Data;

using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Bulk.Api.Storage
{
    public class FileStorage : IStorage
    {
        private readonly IDataTransferObject _data;

        public FileStorage(string path, IConfigParser parser)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                _data = parser.Parse(file);
            }
        }

        public void Dispose()
        {
        }

        public IQuery GetReadAccess()
        {
            return new ObjectQuery(_data);
        }

        public DataConnection GetWriteAccess()
        {
            throw new NotSupportedException();
        }

        private class ObjectQuery : IQuery
        {
            private readonly IDataTransferObject _data;

            public ObjectQuery(IDataTransferObject data)
            {
                _data = data;
            }

            public IQueryable For(Type objType)
            {
                return objType == _data.GetType()
                           ? new[] { _data }.AsQueryable()
                           : new object[0].AsQueryable();
            }

            public IQueryable<T> For<T>() where T : class
            {
                return typeof(T) == _data.GetType()
                           ? new[] { _data }.AsQueryable().Cast<T>()
                           : new T[0].AsQueryable();
            }

            public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
            {
                return For<T>().Where(findSpecification);
            }
        }
    }
}