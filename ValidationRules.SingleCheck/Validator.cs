using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.SingleCheck.Store;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.SingleCheck
{
    public sealed class Validator : IDisposable
    {
        private readonly Pipeline _pipeline;
        private readonly IQuery _sourceQuery;
        private readonly IQuery _pipelineQuery;
        private readonly IStore _pipelineStore;
        private readonly IQuery _targetQuery;
        private readonly IStore _targetStore;

        public Validator(Pipeline pipeline, IStoreFactory sourceFactory, IStoreFactory piplineFactory, IStoreFactory targetFactory)
        {
            _pipeline = pipeline;
            _sourceQuery = sourceFactory.CreateQuery();
            _pipelineQuery = piplineFactory.CreateQuery();
            _pipelineStore = piplineFactory.CreateStore();
            _targetQuery = targetFactory.CreateQuery();
            _targetStore = targetFactory.CreateStore();
        }

        public IReadOnlyCollection<Version.ValidationResult> Execute()
        {
            _pipeline.Execute(_sourceQuery, _pipelineStore, _pipelineQuery, _targetStore);
            return _targetQuery.For<Version.ValidationResult>().ToArray();
        }

        public void Dispose()
        {
            foreach (var disposable in new object[] {_sourceQuery, _pipelineQuery, _targetQuery, _pipelineStore, _targetStore}.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}