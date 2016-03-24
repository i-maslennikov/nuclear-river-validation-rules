using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.OData.Query;

using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace NuClear.Querying.Edm.Tests
{
    public abstract class BaseFixture
    {
        protected static readonly ODataQuerySettings DefaultQuerySettings = new ODataQuerySettings
                                                                          {
                                                                              EnableConstantParameterization = false,
                                                                              HandleNullPropagation = HandleNullPropagationOption.False
                                                                          };

        protected readonly static ODataValidationSettings DefaultValidationSettings = new ODataValidationSettings();

        protected static ODataQueryOptions CreateValidQueryOptions<T>(IEdmModel model, string filter = null)
        {
            return CreateValidQueryOptions(model, typeof(T), filter);
        }

        protected static ODataQueryOptions CreateValidQueryOptions(IEdmModel model, Type entityType, string filter = null)
        {
            var options = CreateQueryOptions(model, entityType, filter);
            try
            {
                options.Validate(DefaultValidationSettings);
            }
            catch (ODataException e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return options;
        }

        protected static ODataQueryOptions CreateQueryOptions<T>(IEdmModel model, string query = null)
        {
            return CreateQueryOptions(model, typeof(T), query);
        }

        protected static ODataQueryOptions CreateQueryOptions(IEdmModel model, Type entityType, string query = null)
        {
            var request = TestHelper.CreateRequest(query);
            var queryOptions = TestHelper.CreateQueryOptions(model, entityType, request);
            return queryOptions;
        }

        protected static IQueryable<T> CreateDataSource<T>()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        protected static IQueryable CreateDataSource(Type type)
        {
            return ((IEnumerable)EnumerableTypeInfo.EmptyMethodInfo.MakeGenericMethod(type).Invoke(null, new object[0])).AsQueryable();
        }

        protected static string ToExpression(IQueryable queryable, string @namespace)
        {
            var expression = queryable.Expression.ToString();
            
            // just reduce the text size
            expression = expression.Replace(@namespace + ".", "").Replace("\"", "'");

            return expression;
        }

        protected static string ToExpression<T>(IQueryable queryable)
        {
            return ToExpression(queryable, typeof(T).Namespace);
        }

        private static class EnumerableTypeInfo
        {
            public static readonly MethodInfo EmptyMethodInfo = typeof(Enumerable).GetMethod("Empty", BindingFlags.Public | BindingFlags.Static);
        }
    }
}