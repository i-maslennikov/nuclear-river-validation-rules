using System;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DataTest.Engine;
using NuClear.DataTest.Engine.Command;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.DI;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure;

using NUnit.Framework;

using ContextEntityTypesProvider = NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure.ContextEntityTypesProvider;
using CreateDatabaseSchemataCommand = NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure.CreateDatabaseSchemataCommand;

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

            var testCasesMetadataSource = new TestCaseMetadataSource();
            var requiredContexts = testCasesMetadataSource.Tests
                                                          .SelectMany(x => x.Arrange.Contexts)
                                                          .Distinct();
            var schemasMetadataSource = new SchemaMetadataSource(requiredContexts);

            _metadataProvider =
                new MetadataProvider(
                    new IMetadataSource[] { schemasMetadataSource, testCasesMetadataSource },
                    new IMetadataProcessor[0]);

            _container.RegisterType<ConnectionStringSettingsAspect, RunnerConnectionStringSettings>()
                      .RegisterType<DataConnectionFactory>()
                      .RegisterInstance<IMetadataProvider>(_metadataProvider)
                      .RegisterType<IContextEntityTypesProvider, ContextEntityTypesProvider>();

            var dropDatabases = _container.Resolve<DropDatabasesCommand>();
            var createDatabases = _container.Resolve<CreateDatabasesCommand>();
            var createSchemata = _container.Resolve<CreateDatabaseSchemataCommand>();

            dropDatabases.Execute();
            createDatabases.Execute();
            createSchemata.Execute();

            _testRunner = _container.Resolve<TestRunner>();
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TestCaseSource(typeof(TestCaseMetadataSource), nameof(TestCaseMetadataSource.Tests))]
        public void Test(TestCaseMetadataElement testCase)
        {
            Assume.That(testCase != null);
            Assume.That(!testCase.Arrange.IsIgnored);

            _testRunner.Execute(testCase);
        }
    }
}
