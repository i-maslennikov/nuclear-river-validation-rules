using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using LinqToDB.Data;
using LinqToDB.Linq;

namespace NuClear.ValidationRules.Storage
{
    /// <summary>
    /// Позволяет получить подсказки sql сервера по созданию индексов.
    /// </summary>
    public sealed class IndexHelper
    {
        private readonly DataConnection _dataConnection;

        public IndexHelper(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<MissingIndex> GetMissingIndices<T>(IQueryable<T> queryable)
        {
            var linq2dbQueryable = queryable as IExpressionQuery<T>;
            if (linq2dbQueryable != null)
            {
                return GetMissingIndices(linq2dbQueryable.ToString());
            }

            var inner = queryable.GetType().GetField("_innerQueryable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (inner != null)
            {
                return GetMissingIndices((IQueryable<T>)inner.GetValue(queryable));
            }

            throw new ArgumentException($"Unsupported queryable type {queryable.GetType().Name}", nameof(queryable));
        }

        public IReadOnlyCollection<MissingIndex> GetMissingIndices(string sql)
        {
            try
            {
                _dataConnection.Execute("SET SHOWPLAN_XML ON");
                var plan = _dataConnection.Execute<XDocument>(sql);
                return plan.Descendants().Where(x => x.Name.LocalName == "MissingIndex").Select(MissingIndex.FromXml).ToArray();
            }
            finally
            {
                _dataConnection.Execute("SET SHOWPLAN_XML OFF");
            }
        }

        public sealed class MissingIndex
        {
            public static MissingIndex FromXml(XElement index)
            {
                return new MissingIndex
                    {
                        Schema = index.Attribute("Schema").Value,
                        Table = index.Attribute("Table").Value,
                        Columns = ColumnNames(index.Elements(), "EQUALITY").Concat(ColumnNames(index.Elements(), "INEQUALITY")).ToArray(),
                        Include = ColumnNames(index.Elements(), "INCLUDE").ToArray(),
                    };
            }

	        private static IEnumerable<string> ColumnNames(IEnumerable<XElement> elements, string usage)
		        => elements
			        .Where(x => x.Name.LocalName == "ColumnGroup" && x.Attribute("Usage").Value == usage)
			        .Elements()
			        .Select(x => x.Attribute("Name").Value);

			private IReadOnlyCollection<string> Include { get; set; }

            private IReadOnlyCollection<string> Columns { get; set; }

            private string Table { get; set; }

            private string Schema { get; set; }

            public override string ToString()
                => $"CREATE NONCLUSTERED INDEX [<IndexName>] ON {Schema}.{Table}({string.Join(", ", Columns)}) INCLUDE({string.Join(", ", Include)})";
        }

    }
}