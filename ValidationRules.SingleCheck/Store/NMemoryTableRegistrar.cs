using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB.Mapping;

using NMemory.Modularity;
using NMemory.Tables;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public class NMemoryTableRegistrar : INMemoryTableRegistrar
    {
        private readonly MappingSchema _mappingSchema;

        public NMemoryTableRegistrar(MappingSchema mappingSchema)
        {
            _mappingSchema = mappingSchema;
        }

        public ITable<T> RegisterTable<T>(IDatabase database) where T : class
        {
            return (ITable<T>)NMemoryTableRegistrator.RegisterLinqToDbMappingSchema(database, _mappingSchema, typeof(T));
        }

        private static class NMemoryTableRegistrator
        {
            private static readonly MethodInfo MethodInfo;
            private static readonly IReadOnlyDictionary<int, Type> TupleTypes = new Dictionary<int, Type>
                {
                        {1, typeof(Tuple<>)},
                        {2, typeof(Tuple<,>)},
                        {3, typeof(Tuple<,,>)},
                        {4, typeof(Tuple<,,,>)},
                        {5, typeof(Tuple<,,,,>)},
                        {6, typeof(Tuple<,,,,,>)},
                        {7, typeof(Tuple<,,,,,,>)},
                        {8, typeof(Tuple<,,,,,,,>)}
                };

            static NMemoryTableRegistrator()
            {
                Expression<Func<TableCollection, ITable>> expression = x => x.Create((Expression<Func<string, long>>)null, null);
                MethodInfo = ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition();
            }

            public static object RegisterLinqToDbMappingSchema(IDatabase database, MappingSchema mappingSchema, Type dataObjectType)
            {
                var primaryKeys = GetPrimaryKeys(mappingSchema, dataObjectType);
                var tupleType = GetTupleTypeRecursive(new ArraySegment<Type>(primaryKeys.Select(x => x.PropertyType).ToArray()));

                var tupleExpression = GetTupleExpression(dataObjectType, tupleType, primaryKeys);
                var createMethod = MethodInfo.MakeGenericMethod(dataObjectType, tupleType);

                // _database.Tables.Create<TEntity, Tuple<long, long>>(x => new Tuple<long, long>(x.Key1, x.Key2), null);
                return createMethod.Invoke(database.Tables, new object[] { tupleExpression, null });
            }

            private static PropertyInfo[] GetPrimaryKeys(MappingSchema mappingSchema, Type dataObjectType)
            {
                var allProperties = dataObjectType.GetProperties();
                var declaredPrimaryKeys = allProperties.Where(x => mappingSchema.GetAttribute<PrimaryKeyAttribute>(x) != null).ToArray();

                return declaredPrimaryKeys.Any() ? declaredPrimaryKeys : allProperties;
            }

            private static Type GetTupleTypeRecursive(ArraySegment<Type> types)
            {
                if (types.Count <= 0)
                {
                    throw new ArgumentException(nameof(types));
                }

                Type tupleType;
                if (types.Count <= 7) // почему-то нельзя создавать tuple на 8 элементов, тьуьщкн ругается
                {
                    var temp = new Type[types.Count];
                    Array.Copy(types.Array, types.Offset, temp, 0, types.Count);
                    tupleType = TupleTypes[types.Count].MakeGenericType(temp);
                }
                else
                {
                    var childTupleType = GetTupleTypeRecursive(new ArraySegment<Type>(types.Array, types.Offset + 7, types.Count - 7));

                    var temp = new Type[8];
                    Array.Copy(types.Array, types.Offset, temp, 0, 7);
                    temp[7] = childTupleType;

                    tupleType = TupleTypes[8].MakeGenericType(temp);
                }

                return tupleType;
            }

            private static Expression GetTupleExpression(Type dataObjectType, Type tupleType, PropertyInfo[] primaryKeys)
            {
                var parameterExpr = Expression.Parameter(dataObjectType);

                var newTupleExpr = GetCreateTupleExpressionRecursive(tupleType, new ArraySegment<PropertyInfo>(primaryKeys), parameterExpr);

                var lambdaExpr = Expression.Lambda(newTupleExpr, parameterExpr);
                return lambdaExpr;
            }

            private static NewExpression GetCreateTupleExpressionRecursive(Type tupleType, ArraySegment<PropertyInfo> propertyInfos, ParameterExpression parameterExpr)
            {
                var args = tupleType.GetGenericArguments();

                var expressions = new Expression[args.Length];
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    var isTuple = arg.IsGenericType && TupleTypes.Values.Contains(arg.GetGenericTypeDefinition());
                    if (!isTuple)
                    {
                        expressions[i] = Expression.Property(parameterExpr, propertyInfos.Array[propertyInfos.Offset + i]);
                    }
                    else
                    {
                        var segment = new ArraySegment<PropertyInfo>(propertyInfos.Array, propertyInfos.Offset + i, propertyInfos.Count - i);
                        expressions[i] = GetCreateTupleExpressionRecursive(arg, segment, parameterExpr);
                    }
                }

                var newTupleExpr = Expression.New(tupleType.GetConstructors()[0], expressions);
                return newTupleExpr;
            }
        }
    }
}