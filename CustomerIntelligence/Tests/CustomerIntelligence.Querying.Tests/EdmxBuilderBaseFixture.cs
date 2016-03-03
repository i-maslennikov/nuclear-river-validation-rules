using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Reflection;

using Effort;
using Effort.DataLoaders;
using Effort.Provider;

using Moq;

using NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm.EF;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class EdmxBuilderBaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        protected static DbProviderInfo EffortProvider
        {
            get
            {
                return new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
            }
        }

        protected DbModel CreateModel()
        {
            return BuildModel(TestMetadataProvider.Instance, CustomerIntelligenceTypeResolver);
        }

        private static IClrTypeBuilder CustomerIntelligenceTypeResolver
        {
            get
            {
                return MockTypeProvider(
                    typeof(CategoryGroup),
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
            }
        }

        private static IClrTypeBuilder MockTypeProvider(params Type[] types)
        {
            var typeProvider = new Mock<IClrTypeBuilder>();

            foreach (var type in types)
            {
                RegisterType(typeProvider, type);
            }

            return typeProvider.Object;
        }

        private static void RegisterType(Mock<IClrTypeBuilder> typeProvider, Type type)
        {
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == type.Name))).Returns(type);
        }

        private static DbModel BuildModel(IMetadataProvider metadataProvider, IClrTypeBuilder clrTypeBuilder = null)
        {
            var builder = CreateBuilder(metadataProvider, clrTypeBuilder);
            var contextId = BuildContextId();
            return builder.Build(contextId, EffortProvider);
        }

        // NOTE: Assembly name CANNOT start with digits, for example, 2GIS.Assembly.Name. It should be just, for example, Assembly.Name or DoubleGIS.Assembly.Name
        private static readonly string DefaultTestDataUri = string.Format("res://{0}/Data", Assembly.GetExecutingAssembly().GetName().Name);

        protected static DbConnection CreateConnection(string path = null)
        {
            var dataLoader = new CsvDataLoader(path ?? DefaultTestDataUri);
            var cachingLoader = new CachingDataLoader(dataLoader);
            return DbConnectionFactory.CreateTransient(cachingLoader);
        }

        private static EdmxModelBuilder CreateBuilder(IMetadataProvider metadataProvider, IClrTypeBuilder clrTypeBuilder = null)
        {
            return clrTypeBuilder == null
                ? new EdmxModelBuilder(metadataProvider)
                : new EdmxModelBuilder(metadataProvider, clrTypeBuilder);
        }

        private static Uri BuildContextId()
        {
            return QueryingMetadataIdentity.Instance.Id.WithRelative(new Uri("CustomerIntelligence", UriKind.Relative));
        }
    }
}