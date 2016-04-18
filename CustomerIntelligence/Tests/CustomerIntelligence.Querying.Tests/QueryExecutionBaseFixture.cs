using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.OData;
using System.Web.OData.Query;

using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public abstract class QueryExecutionBaseFixture
    {
        protected static readonly ODataQuerySettings DefaultQuerySettings = new ODataQuerySettings
        {
            EnableConstantParameterization = false,
            HandleNullPropagation = HandleNullPropagationOption.False
        };

        protected static readonly ODataValidationSettings DefaultValidationSettings = new ODataValidationSettings();

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

        protected static ODataQueryOptions CreateQueryOptions(IEdmModel model, Type entityType, string query = null)
        {
            var request = CreateRequest(query);
            var queryOptions = CreateQueryOptions(model, entityType, request);
            return queryOptions;
        }

        protected static IQueryable CreateDataSource(Type type)
        {
            return ((IEnumerable)EnumerableTypeInfo.EmptyMethodInfo.MakeGenericMethod(type).Invoke(null, new object[0])).AsQueryable();
        }

        protected static string ToExpression(IQueryable queryable, string @namespace)
        {
            var expression = queryable.Expression.ToString();

            // just reduce the text size
            expression = expression.Replace(@namespace + ".", string.Empty).Replace("\"", "'");

            return expression;
        }

        private static HttpRequestMessage CreateRequest(string query = null)
        {
            var uriBuilder = new UriBuilder();
            if (query != null)
            {
                uriBuilder.Query = query;
            }

            return new HttpRequestMessage { RequestUri = uriBuilder.Uri };
        }

        private static ODataQueryOptions CreateQueryOptions(IEdmModel model, Type elementClrType, HttpRequestMessage request)
        {
            var context = new ODataQueryContext(model, elementClrType, null);
            var queryOptions = new ODataQueryOptions(context, request);
            return queryOptions;
        }

        private static class EnumerableTypeInfo
        {
            public static readonly MethodInfo EmptyMethodInfo = typeof(Enumerable).GetMethod("Empty", BindingFlags.Public | BindingFlags.Static);
        }
    }
}