using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.SingleCheck.Optimization;
using NuClear.ValidationRules.SingleCheck.Store;

namespace NuClear.ValidationRules.SingleCheck
{
    internal abstract class Replicator
    {
        private static readonly ConcurrentDictionary<Type, AccessorInfo> Cache
            = new ConcurrentDictionary<Type, AccessorInfo>();

        public static Replicator Create(Type accessorType, Type dataObjectType, IQuery source, IStore target)
        {
            var genericReplicatorType = typeof(ReplicatorImpl<>).MakeGenericType(dataObjectType);
            var accessor = Activator.CreateInstance(accessorType, source);
            return (Replicator)Activator.CreateInstance(genericReplicatorType, accessor, target);
        }

        public abstract void Process(bool isEmpty);
        public abstract IReadOnlyCollection<LambdaExpression> DependencyPredicates { get; }
        public abstract Func<Func<Type, LambdaExpression, bool>, bool> SkipCheckFunction { get; }
        public abstract IReadOnlyCollection<Type> Dependencies { get; }
        public abstract Type DataObjectType { get; }

        private static AccessorInfo GetAccessorInfo(Type accessorType, Lazy<Expression> expression)
            => Cache.GetOrAdd(accessorType, t => ParseExpression(expression.Value));

        private static AccessorInfo ParseExpression(Expression expression)
        {
            var checkVisitor = new SkipCheckVisitor();
            var checkExpression = checkVisitor.Visit(expression);
            var skipCheckFunction = Expression.Lambda<Func<Func<Type, LambdaExpression, bool>, bool>>(checkExpression, checkVisitor.Parameter).Compile();

            var dependencyVisitor = new DependencyVisitor();
            dependencyVisitor.Visit(expression);

            return new AccessorInfo(skipCheckFunction, checkVisitor.DependencyPredicates, dependencyVisitor.Dependencies);
        }

        private sealed class ReplicatorImpl<TDataType> : Replicator
            where TDataType : class
        {
            private readonly AccessorInfo _info;
            private readonly IStorageBasedDataObjectAccessor<TDataType> _accessor;
            private readonly IStore _target;

            public ReplicatorImpl(IStorageBasedDataObjectAccessor<TDataType> accessor, IStore target)
            {
                _info = GetAccessorInfo(accessor.GetType(), new Lazy<Expression>(() => accessor.GetSource().Expression));
                _accessor = accessor;
                _target = target;
            }

            public override IReadOnlyCollection<LambdaExpression> DependencyPredicates => _info.DependencyPredicates;

            public override Func<Func<Type, LambdaExpression, bool>, bool> SkipCheckFunction => _info.SkipCheckFunction;

            public override IReadOnlyCollection<Type> Dependencies => _info.Dependencies;

            public override Type DataObjectType => typeof(TDataType);

            public override void Process(bool isEmpty)
            {
                TDataType[] data;
                if (isEmpty)
                {
                    data = Array.Empty<TDataType>();
                }
                else
                {
                    data = _accessor.GetSource().Execute();
                }

                _target.AddRange(data);
            }
        }

        private sealed class AccessorInfo
        {
            public AccessorInfo(Func<Func<Type, LambdaExpression, bool>, bool> skipCheckFunction, IReadOnlyCollection<LambdaExpression> dependencyPredicates, IReadOnlyCollection<Type> dependencies)
            {
                SkipCheckFunction = skipCheckFunction;
                DependencyPredicates = dependencyPredicates;
                Dependencies = dependencies;
            }

            public Func<Func<Type, LambdaExpression, bool>, bool> SkipCheckFunction { get; }
            public IReadOnlyCollection<LambdaExpression> DependencyPredicates { get; }
            public IReadOnlyCollection<Type> Dependencies { get; }
        }
    }
}