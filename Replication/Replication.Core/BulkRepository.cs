using System.Collections.Generic;

using NuClear.Replication.Core.API.DataObjects;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core
{
    public class BulkRepository<TDataObject> : IBulkRepository<TDataObject>
        where TDataObject : class
    {
        private readonly IRepository<TDataObject> _repository;

        public BulkRepository(IRepository<TDataObject> repository)
        {
            _repository = repository;
        }

        public void Create(IEnumerable<TDataObject> objects)
        {
            using (Probe.Create("Inserting", typeof(TDataObject).Name))
            {
                _repository.AddRange(objects);
                _repository.Save();
            }
        }

        public void Update(IEnumerable<TDataObject> objects)
        {
            using (Probe.Create("Updating", typeof(TDataObject).Name))
            {
                foreach (var obj in objects)
                {
                    _repository.Update(obj);
                }

                _repository.Save();
            }
        }

        public void Delete(IEnumerable<TDataObject> objects)
        {
            using (Probe.Create("Deleting", typeof(TDataObject).Name))
            {
                _repository.DeleteRange(objects);
                _repository.Save();
            }
        }
    }
}