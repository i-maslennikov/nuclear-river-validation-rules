using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.ValidationRules.Replication
{
    public abstract class AggregateRootActor<TRootEntity> : IAggregateRootActor
        where TRootEntity : class
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        private EntityActor<TRootEntity> _root;

        protected AggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
            => _root.ExecuteCommands(commands);

        public IReadOnlyCollection<IActor> GetValueObjectActors()
            => _root.GetValueObjectActors();

        public Type EntityType
            => _root.EntityType;

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        protected void HasRootEntity<TAccessor>(TAccessor accessor, IBulkRepository<TRootEntity> repository, params IActor[] valueObjectActors)
            where TAccessor : IStorageBasedDataObjectAccessor<TRootEntity>, IDataChangesHandler<TRootEntity>
        {
            _root = new EntityActor<TRootEntity>(_query, repository, _equalityComparerFactory, accessor, accessor, valueObjectActors);
        }

        protected IActor HasValueObject<TEntity, TAccessor>(TAccessor accessor, IBulkRepository<TEntity> repository)
            where TAccessor : IStorageBasedDataObjectAccessor<TEntity>, IDataChangesHandler<TEntity>
            where TEntity : class
        {
            return new ValueObjectActor<TEntity>(new ValueObjectChangesProvider<TEntity>(_query, accessor, _equalityComparerFactory), repository, accessor);
        }

        private sealed class EntityActor<TEntity> : EntityActorBase<TEntity> where TEntity : class
        {
            private readonly IReadOnlyCollection<IActor> _valueObjectActors;

            public EntityActor(
                IQuery query,
                IBulkRepository<TEntity> bulkRepository,
                IEqualityComparerFactory equalityComparerFactory,
                IStorageBasedDataObjectAccessor<TEntity> storageBasedDataObjectAccessor,
                IDataChangesHandler<TEntity> dataChangesHandler,
                IReadOnlyCollection<IActor> valueObjectActors)
                : base(query, bulkRepository, equalityComparerFactory, storageBasedDataObjectAccessor, dataChangesHandler)
            {
                _valueObjectActors = valueObjectActors;
            }

            public override IReadOnlyCollection<IActor> GetValueObjectActors()
            {
                return _valueObjectActors;
            }
        }
    }
}