using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinqToDB.Mapping;

using NuClear.Replication.Core.Equality;

namespace NuClear.ValidationRules.Storage
{
    public sealed class LinqToDbPropertyProvider : IObjectPropertyProvider
    {
        private readonly MappingSchema _schema;

        public LinqToDbPropertyProvider(params MappingSchema[] schemata)
        {
            _schema = new MappingSchema(schemata);
        }

        public IReadOnlyCollection<PropertyInfo> GetPrimaryKeyProperties<T>()
        {
            var keyProperties = FindProperties(typeof(T), x => x.IsPrimaryKey);
            return keyProperties.Any() ? keyProperties : GetProperties<T>();
        }

        public IReadOnlyCollection<PropertyInfo> GetProperties<T>()
        {
            return FindProperties(typeof(T), x => true);
        }

        private IReadOnlyCollection<PropertyInfo> FindProperties(Type type, Func<ColumnDescriptor, bool> predicate)
        {
            return _schema.GetEntityDescriptor(type).Columns.Where(predicate).Select(x => x.MemberInfo).Cast<PropertyInfo>().ToArray();
        }
    }
}