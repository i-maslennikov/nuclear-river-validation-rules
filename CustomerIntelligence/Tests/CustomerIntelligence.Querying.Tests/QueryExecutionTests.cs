using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm;
using NuClear.Querying.Edm.Emit;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    /// <remarks>
    /// Executes in invariant culture to simplify expected result after the formatting.
    /// </remarks>>
    [TestFixture, SetCulture("")]
    public sealed class QueryExecutionTests : QueryExecutionBaseFixture
    {
        private readonly IDictionary<Uri, IEdmModel> _models = BuildModels(TestMetadataProvider.Instance, BusinessDirectoryId, CustomerIntelligenceId);

        [TestCase("CategoryGroup", null, Result = "CategoryGroup[]")]
        [TestCase("Project", null, Result = "Project[]")]
        public string ShouldBuildQuery(string type, string filter)
        {
            return BuildQuery(CustomerIntelligence, type, filter);
        }

        /// <summary>
        /// Критерии взяты из https://confluence.2gis.ru/pages/viewpage.action?pageId=143462711.
        /// </summary>
        [TestCase("Firm", "$filter=CreatedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.CreatedOn < 01/01/2015 00:00:00 +00:00))", Description = "Поиск по дате создания.")]
        [TestCase("Firm", "$filter=LastDisqualifiedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDisqualifiedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего возвращения в резерв.")]
        [TestCase("Firm", "$filter=LastDistributedOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastDistributedOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего размещения.")]
        [TestCase("Firm", "$filter=LastActivityOn lt @date&@date=2015-01-01T00:00Z", Result = "Firm[].Where($it => ($it.LastActivityOn < Convert(01/01/2015 00:00:00 +00:00)))", Description = "Поиск по дате последнего действия.")]
        [TestCase("Firm", "$filter=HasPhone eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasPhone == True))", Description = "Поиск по наличию телефона.")]
        [TestCase("Firm", "$filter=HasWebsite eq @value&@value=true", Result = "Firm[].Where($it => ($it.HasWebsite == True))", Description = "Поиск по наличию сайта.")]
        [TestCase("Firm", "$filter=AddressCount gt @amount&@amount=10", Result = "Firm[].Where($it => ($it.AddressCount > 10))", Description = "Поиск по количеству активных адресов.")]
        [TestCase("Firm", "$filter=CategoryGroup/Id eq @id&@id=12345", Result = "Firm[].Where($it => ($it.CategoryGroup.Id == Convert(12345)))", Description = "Поиск по ценовой категории фирмы.")]
        [TestCase("Firm", "$filter=Territories/any(it: (it/TerritoryId eq 123) or (it/TerritoryId eq null))", Result = "Firm[].Where($it => $it.Territories.Any(it => ((it.TerritoryId == Convert(123)) OrElse (it.TerritoryId == null))))", Description = "Поиск по территории.")]
        [TestCase("Firm", "$filter=Categories1/any(x:x/CategoryId eq @id1)&@id1=123", Result = "Firm[].Where($it => $it.Categories1.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 1-го уровня.")]
        [TestCase("Firm", "$filter=Categories2/any(x:x/CategoryId eq @id2)&@id2=123", Result = "Firm[].Where($it => $it.Categories2.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 2-го уровня.")]
        [TestCase("Firm", "$filter=Categories3/any(x:x/CategoryId eq @id3)&@id3=123", Result = "Firm[].Where($it => $it.Categories3.Any(x => (x.CategoryId == Convert(123))))", Description = "Поиск по рубрике 3-го уровня.")]
        [TestCase("Firm", "$filter=Client/CategoryGroup/Id eq @id&@id=12345", Result = "Firm[].Where($it => ($it.Client.CategoryGroup.Id == Convert(12345)))", Description = "Поиск по ценовой категории клиента.")]
        [TestCase("Firm", "$filter=Client/Contacts/any(x:x/Role eq Querying.CustomerIntelligence.ContactRole'Employee')", Result = "Firm[].Where($it => $it.Client.Contacts.Any(x => (Convert(x.Role) == Convert(Employee))))", Description = "Поиск по роли контакта.")]
        [TestCase("Firm", "$filter=Balances/all(x:x/Balance gt @balance)&@balance=1000", Result = "Firm[].Where($it => $it.Balances.All(x => (x.Balance > Convert(1000))))", Description = "Поиск по балансу лицевого счета.")]
        public string ShouldAcceptMainCriteria(string type, string filter)
        {
            return BuildQuery(CustomerIntelligence, type, filter);
        }

        private string BuildQuery(string modelName, string type, string filter)
        {
            var model = _models[Metadata.Id.For<QueryingMetadataIdentity>(modelName)];

            var firmType = LookupClrType(model, type);

            var options = CreateValidQueryOptions(model, firmType, filter);

            var query = options.ApplyTo(CreateDataSource(firmType), DefaultQuerySettings);

            var expression = ToExpression(query, firmType.Namespace);

            Debug.WriteLine((string)expression);

            return expression;
        }

        private static IReadOnlyDictionary<IMetadataElementIdentity,Type> EmitClrTypes(BoundedContextElement context)
        {
            var typeProvider = new EmitTypeProvider();
            foreach (var element in context.ConceptualModel.Entities)
            {
                typeProvider.Resolve(element);
            }
            return typeProvider.RegisteredTypes;
        }

        private static Type LookupClrType(IEdmModel model, string name)
        {
            var @namespace = model.DeclaredNamespaces.SingleOrDefault();
            var fullName = (string.IsNullOrEmpty(@namespace) ? "" : @namespace + ".") + name;

            var edmType = model.FindDeclaredType(fullName);
            if (edmType == null)
            {
                throw new Exception("The type was not found for the specified name.");
            }

            var annotation = model.GetAnnotationValue<ClrTypeAnnotation>(edmType);
            if (annotation == null)
            {
                throw new Exception("The CLR type cannot be resolved.");
            }

            return annotation.ClrType;
        }

        private static IDictionary<Uri, IEdmModel> BuildModels(IMetadataProvider provider, params Uri[] identities)
        {
            var models = new Dictionary<Uri, IEdmModel>();
            var builder = new EdmModelBuilder(provider);

            foreach (var identity in identities)
            {
                BoundedContextElement context;
                provider.TryGetMetadata(identity, out context);

                if (context != null)
                {
                    var clrTypes = EmitClrTypes(context);
                    var model = builder.Build(identity, clrTypes.Values);

                    models.Add(identity, model);
                }
            }

            return models;
        }

        private const string BusinessDirectory = "BusinessDirectory";
        private const string CustomerIntelligence = "CustomerIntelligence";

        private static readonly Uri BusinessDirectoryId = Metadata.Id.For<QueryingMetadataIdentity>(BusinessDirectory);
        private static readonly Uri CustomerIntelligenceId = Metadata.Id.For<QueryingMetadataIdentity>(CustomerIntelligence);
    }
}