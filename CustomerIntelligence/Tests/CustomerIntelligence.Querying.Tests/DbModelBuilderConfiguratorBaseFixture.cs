using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

using Effort;
using Effort.DataLoaders;
using Effort.Provider;

using Moq;

using NuClear.CustomerIntelligence.Querying.Tests.Model;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm.EF;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Identities;
using NuClear.Querying.Storage;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class DbModelBuilderConfiguratorBaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        protected static DbProviderInfo EffortProvider => new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);

        protected DbModel CreateModel()
        {
            return BuildModel(TestMetadataProvider.Instance, CustomerIntelligenceTypeResolver);
        }

        private static IClrTypeProvider CustomerIntelligenceTypeResolver =>
            MockTypeProvider(typeof(CategoryGroup),
                             typeof(Client),
                             typeof(ClientContact),
                             typeof(Firm),
                             typeof(FirmBalance),
                             typeof(FirmCategory1),
                             typeof(FirmCategory2),
                             typeof(FirmCategory3),
                             typeof(FirmTerritory),
                             typeof(Project),
                             typeof(Category),
                             typeof(Territory));

        private static IClrTypeProvider MockTypeProvider(params Type[] types)
        {
            var typeProvider = new Mock<IClrTypeProvider>();

            foreach (var type in types)
            {
                RegisterType(typeProvider, type);
            }

            return typeProvider.Object;
        }

        private static void RegisterType(Mock<IClrTypeProvider> typeProvider, Type type)
        {
            typeProvider.Setup(x => x.Get(It.Is<IMetadataElementIdentity>(el => el.Id.Segments.Last() == type.Name))).Returns(type);
        }

        private static DbModel BuildModel(IMetadataProvider metadataProvider, IClrTypeProvider clrTypeProvider)
        {
            BoundedContextElement context;
            metadataProvider.TryGetMetadata(BuildContextId(), out context);

            var configurator = new DbModelBuilderConfigurator(metadataProvider, clrTypeProvider);
            var builder = configurator.Configure(context);

            return builder.Build(EffortProvider);
        }

        // NOTE: Assembly name CANNOT start with digits, for example, 2GIS.Assembly.Name. It should be just, for example, Assembly.Name or DoubleGIS.Assembly.Name
        private static readonly string DefaultTestDataUri = $"res://{Assembly.GetExecutingAssembly().GetName().Name}/Data";

        protected static DbConnection CreateConnection(string path = null)
        {
            var dataLoader = new CsvDataLoader(path ?? DefaultTestDataUri);
            var cachingLoader = new CachingDataLoader(dataLoader);
            return DbConnectionFactory.CreateTransient(cachingLoader);
        }

        private static Uri BuildContextId()
        {
            return QueryingMetadataIdentity.Instance.Id.WithRelative(new Uri("CustomerIntelligence", UriKind.Relative));
        }
    }
}