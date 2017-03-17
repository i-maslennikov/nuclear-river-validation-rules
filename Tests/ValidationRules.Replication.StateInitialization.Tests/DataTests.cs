using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DataTest.Engine;
using NuClear.DataTest.Engine.Command;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.DI;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    [TestFixture]
    public sealed class DataTests
    {
        private UnityContainer _container;
        private MetadataProvider _metadataProvider;
        private TestRunner _testRunner;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            StateInitializationTestsRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true, typeof(object));
            _container = new UnityContainer();
            _metadataProvider =
                new MetadataProvider(
                    new IMetadataSource[] { new SchemaMetadataSource(), new TestCaseMetadataSource() },
                    new IMetadataProcessor[0]);

            _container.RegisterType<ConnectionStringSettingsAspect, RunnerConnectionStringSettings>();
            _container.RegisterType<DataConnectionFactory>();
            _container.RegisterInstance<IMetadataProvider>(_metadataProvider);

#if !DEBUG
            var dropDatabases = _container.Resolve<DropDatabasesCommand>();
            var createDatabases = _container.Resolve<CreateDatabasesCommand>();
            var createSchemata = _container.Resolve<CreateDatabaseSchemataCommand>();

            dropDatabases.Execute();
            createDatabases.Execute();
            createSchemata.Execute();
#endif

            _testRunner = _container.Resolve<TestRunner>();
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TestCaseSource(typeof(TestCaseMetadataSource), "Metadata")]
        public void Test(KeyValuePair<Uri, IMetadataElement> element)
        {
            var test = element.Value as TestCaseMetadataElement;

            Assume.That(test != null);
            Assume.That(!test.Arrange.IsIgnored);

            _testRunner.Execute(test);
        }
    }
}
