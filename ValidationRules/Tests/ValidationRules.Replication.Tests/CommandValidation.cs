using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain;
using NuClear.ValidationRules.Domain.Model;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    public class CommandValidation<TFact>
        where TFact : class, IIdentifiable<long>
    {
        private readonly Store _store;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _dependencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;
        private readonly Func<TFact, long> _identityExtractor;
        private IEnumerable<IOperation> _commands;

        public CommandValidation(IReadOnlyCollection<object> data, FactMetadata<TFact> metadata)
        {
            _store = new Store(data);
            _dependencyProcessors = metadata.Features.OfType<IFactDependencyFeature>().Select(Create).ToArray();
            _indirectDepencencyProcessors = metadata.Features.OfType<IIndirectFactDependencyFeature>().Select(Create).ToArray();
            _identityExtractor = new DefaultIdentityProvider().ExtractIdentity<TFact>().Compile();
        }

        public static CommandValidation<TFact> Given(params object[] data)
        {
            var source = new FactsReplicationMetadataSource();
            var metadata = source.Metadata.Values.OfType<HierarchyMetadata>().Single().Elements.OfType<FactMetadata<TFact>>().Single();
            return new CommandValidation<TFact>(data, metadata);
        }

        private IFactDependencyProcessor Create(IFactDependencyFeature feature)
            => new FactDependencyProcessor<TFact>((IFactDependencyFeature<TFact, long>)feature, _store.GetQuery());

        public CommandValidation<TFact> Create(TFact fact)
        {
            _store.Create(fact);
            _commands = _dependencyProcessors.SelectMany(x => x.ProcessCreation(new[] { _identityExtractor.Invoke(fact) })).ToArray();
            return this;
        }

        public CommandValidation<TFact> Update(TFact fact)
        {
            _commands = _indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(new[] { _identityExtractor.Invoke(fact) })).ToArray();
            _store.Update(fact);
            _commands = _commands.Concat(_dependencyProcessors.SelectMany(x => x.ProcessUpdating(new[] { _identityExtractor.Invoke(fact) }))).ToArray();
            return this;
        }

        public CommandValidation<TFact> Delete(TFact fact)
        {
            _commands = _dependencyProcessors.SelectMany(x => x.ProcessDeletion(new[] { _identityExtractor.Invoke(fact) })).ToArray();
            _store.Delete(fact);
            return this;
        }

        public void Expect(params IOperation[] operations)
        {
            Assert.That(_commands, Is.EquivalentTo(operations));
        }

        private class Store
        {
            private readonly HashSet<object> _data;

            public Store(IEnumerable<object> data)
            {
                _data = new HashSet<object>(data, new IdentifiableEqualityComparer());
            }

            public IQuery GetQuery()
            {
                return new Query(_data);
            }

            public void Create(object obj)
            {
                _data.Add(obj);
            }

            public void Update(object obj)
            {
                _data.Add(obj);
            }

            public void Delete(object obj)
            {
                _data.Remove(obj);
            }

            private class Query : IQuery
            {
                private readonly HashSet<object> _data;

                public Query(HashSet<object> data)
                {
                    _data = data;
                }

                public IQueryable For(Type objType)
                {
                    return _data.Where(x => x.GetType() == objType).ToArray().AsQueryable();
                }

                public IQueryable<T> For<T>() where T : class
                {
                    return _data.Where(x => x.GetType() == typeof(T)).Cast<T>().ToArray().AsQueryable();
                }

                public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
                {
                    return _data.Where(x => x.GetType() == typeof(T)).Cast<T>().ToArray().AsQueryable().Where(findSpecification);
                }
            }

            private class IdentifiableEqualityComparer : IEqualityComparer<object>
            {
                private readonly IDictionary<Type, object> _identityProviders = new Dictionary<Type, object>
                    {
                        { typeof(long), new DefaultIdentityProvider() },
                        { typeof(PeriodId), new PeriodIdentityProvider() },
                    };

                bool IEqualityComparer<object>.Equals(object x, object y)
                {
                    if (x.GetType() != y.GetType())
                    {
                        return false;
                    }

                    var xId = GetIdentity(x);
                    var yId = GetIdentity(y);
                    return xId.Equals(yId);
                }

                int IEqualityComparer<object>.GetHashCode(object obj)
                {
                    var id = GetIdentity(obj);
                    return id.GetHashCode();
                }

                private Type GetKeyType(Type type)
                    => type
                        .GetInterfaces()
                        .Single(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IIdentifiable<>))
                        .GetGenericArguments()
                        .First();

                private object GetIdentity(object x)
                    => GetType()
                        .GetMethod("GetGenericIdentity", BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(x.GetType(), GetKeyType(x.GetType()))
                        .Invoke(this, new[] { x });

                // ReSharper disable once UnusedMember.Local
                private TKey GetGenericIdentity<T, TKey>(T x)
                    where T : IIdentifiable<TKey>
                {
                    object obj;
                    if (!_identityProviders.TryGetValue(typeof(TKey), out obj))
                    {
                        throw new Exception($"Для прохождения теста добавь IIdentityProvider<{typeof(TKey)}>");
                    }

                    var identityFunc = ((IIdentityProvider<TKey>)obj).ExtractIdentity<T>().Compile();
                    return identityFunc.Invoke(x);
                }
            }
        }
    }
}