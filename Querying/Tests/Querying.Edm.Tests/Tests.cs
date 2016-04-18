using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.OData.Extensions;
using System.Web.OData.Query;

using Microsoft.OData.Core;

using NUnit.Framework;

namespace NuClear.Querying.Edm.Tests
{
    [TestFixture]
    public sealed class Tests : BaseFixture
    {
        [TestCase("$filter=Id ne 1", Result = "MasterClass[].Where($it => ($it.Id != 1))")]
        [TestCase("$filter=EnumType eq Querying.Context.EnumType'Member1'", Result = "MasterClass[].Where($it => (Convert($it.EnumType) == Convert(Member1)))")]
        [TestCase("$filter=NonExistent ne null", ExpectedException = typeof(ODataException))]
        [TestCase("$orderby=Id desc", Result = "MasterClass[].OrderByDescending($it => $it.Id)")]
        [TestCase("$orderby=NonExistent desc", ExpectedException = typeof(ODataException))]
        [TestCase("$skip=2&$top=1", Result = "MasterClass[].OrderBy($it => $it.Id).Skip(2).Take(1)")]
        [TestCase("$select=Id,Name", Result = "MasterClass[].Select(Param_0 => new SelectSome`1() {ModelID = '<GUID>', Container = new NamedPropertyWithNext`1() {Name = 'Name', Value = Param_0.Name, Next = new NamedProperty`1() {Name = 'Id', Value = Convert(Param_0.Id)}}})")]
        [TestCase("$select=NonExistent", ExpectedException = typeof(ODataException))]
        [TestCase("$expand=NestedClass", Result = "MasterClass[].Select(Param_0 => new SelectAllAndExpand`1() {ModelID = '<GUID>', Instance = Param_0, Container = new SingleExpandedProperty`1() {Name = 'NestedClass', Value = new SelectAll`1() {ModelID = '<GUID>', Instance = Param_0.NestedClass}, IsNull = (Param_0.NestedClass == null)}})")]
        [TestCase("$expand=NonExistent", ExpectedException = typeof(ODataException))]
        public string ShouldBeValidAndEqualTo(string filter)
        {
            var queryOptions = CreateValidQueryOptions<MasterClass>(TestModelSource.EdmModel, filter);

            var queryable = queryOptions.ApplyTo(DataSource, DefaultQuerySettings);

            var expression = ToExpression<MasterClass>(queryable);

            // catch GUIDs
            expression = Regex.Replace(expression, @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b", "<GUID>");

            Debug.WriteLine(expression);

            return expression;
        }

        [TestCase("$count=true", Result = 5)]
        [TestCase("$count=false", Result = null)]
        public long? ShouldCountData(string query)
        {
            var dataSource = Enumerable.Repeat(new MasterClass(), 5).AsQueryable();

            var request = TestHelper.CreateRequest(query);
            var queryOptions = TestHelper.CreateQueryOptions(TestModelSource.EdmModel, typeof(MasterClass), request);

            queryOptions.ApplyTo(dataSource, DefaultQuerySettings);

            return request.ODataProperties().TotalCount;
        }

        [TestCase(5, 2, Result = 2)]
        public int ShouldLimitDataToPageSize(int dataRange, int pageSize)
        {
            var dataSource = Enumerable.Repeat(new MasterClass(), dataRange).AsQueryable();

            var queryOptions = CreateValidQueryOptions<MasterClass>(TestModelSource.EdmModel);
            var queryable = (IQueryable<MasterClass>)queryOptions.ApplyTo(dataSource, new ODataQuerySettings(DefaultQuerySettings) { PageSize = pageSize });

            return queryable.Count();
        }

        private static IQueryable<MasterClass> DataSource => CreateDataSource<MasterClass>();
    }
}
