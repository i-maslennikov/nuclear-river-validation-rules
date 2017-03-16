using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.DataTest.Engine;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.DataTest.Engine.Command;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Replication.AdvertisementRules.Validation;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    [TestFixture]
    public sealed class DataTests
    {
        private readonly AdvertisementElementMustPassReview Anchor = null;

        private UnityContainer _container;
        private MetadataProvider _metadataProvider;
        private TestRunner _testRunner;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _container = new UnityContainer();
            _metadataProvider =
                new MetadataProvider(
                    new IMetadataSource[] { new SchemaMetadataSource(), new TestCaseMetadataSource() },
                    new IMetadataProcessor[0]);

            _container.RegisterType<ConnectionStringSettingsAspect, RunnerConnectionStringSettings>();
            _container.RegisterType<DataConnectionFactory>();
            _container.RegisterInstance<IMetadataProvider>(_metadataProvider);

            //var dropDatabases = _container.Resolve<DropDatabasesCommand>();
            //var createDatabases = _container.Resolve<CreateDatabasesCommand>();
            //var createSchemata = _container.Resolve<CreateDatabaseSchemataCommand>();

            //dropDatabases.Execute();
            //createDatabases.Execute();
            //createSchemata.Execute();

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
            _testRunner.Execute(test);
        }
    }
}
