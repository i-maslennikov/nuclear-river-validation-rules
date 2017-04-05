using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Mapping;

namespace NuClear.ValidationRules.Storage
{
    public static class SchemaExtensions
    {
        public static EntityMappingBuilder<T> HasIndex<T>(this EntityMappingBuilder<T> builder, Expression<Func<T, object>> fields)
        {
            var fieldsVisitor = new Visitor();
            fieldsVisitor.Visit(fields);

            builder.HasAttribute(new IndexAttribute { Fields = fieldsVisitor.Members, Include = Array.Empty<MemberInfo>() });

            return builder;
        }

        public static EntityMappingBuilder<T> HasIndex<T>(this EntityMappingBuilder<T> builder, Expression<Func<T, object>> fields, Expression<Func<T, object>> fieldsInclude)
        {
            var fieldsVisitor = new Visitor();
            fieldsVisitor.Visit(fields);

            var fieldsIncludeVisitor = new Visitor();
            fieldsIncludeVisitor.Visit(fieldsInclude);

            builder.HasAttribute(new IndexAttribute { Fields = fieldsVisitor.Members, Include = fieldsIncludeVisitor.Members });

            return builder;
        }

        public sealed class IndexAttribute : Attribute
        {
            public IReadOnlyCollection<MemberInfo> Fields { get; set; }
            public IReadOnlyCollection<MemberInfo> Include { get; set; }
        }

        private sealed class Visitor : ExpressionVisitor
        {
            private readonly HashSet<MemberInfo> _members = new HashSet<MemberInfo>();

            public IReadOnlyCollection<MemberInfo> Members => _members;

            protected override Expression VisitMember(MemberExpression node)
            {
                _members.Add(node.Member);
                return base.VisitMember(node);
            }
        }
    }
}