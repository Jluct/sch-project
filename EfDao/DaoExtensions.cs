using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;

namespace EfDao
{
    public static class DaoExtensions
    {
        public static string GetEntitySetFullName(this ObjectContext objectContext, string entityTypeName)
        {
            var container = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace);
            var entitySetName = (from meta in container.BaseEntitySets
                                 where meta.ElementType.Name == entityTypeName
                                 select meta.Name).First();
            //return container.Name + "." + entitySetName;
            return entitySetName;
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> elementCollection, FilterParam[] filters, SortParam sortParam)
        {
            var result = elementCollection;
            if (filters != null)
            {
                foreach (var filterParam in filters)
                {
                    var prop = typeof(T).GetProperty(filterParam.Property);
                    FilterParam param = filterParam;
                    if (prop != null)
                        switch (filterParam.Expression)
                        {
                            //TODO: implement correct way of Contains/StartWith/EndWith
                            case DaoExpression.Contains:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.Contains,
                                                     param.Value);
                                break;
                            case DaoExpression.EndsWith:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.EndsWith,
                                                     param.Value);
                                break;
                            case DaoExpression.StartsWith:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.StartsWith,
                                                     param.Value);
                                break;
                            case DaoExpression.Eq:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.Equal,
                                                     param.Value);
                                break;
                            case DaoExpression.Gt:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.GreaterThan,
                                                     param.Value);
                                break;
                            case DaoExpression.GtEq:
                                result = ApplyFilter(result, param.Property,
                                                     ExtendedLinqExpressionType.GreaterThanOrEqual, param.Value);
                                break;
                            case DaoExpression.Lt:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.LessThan,
                                                     param.Value);
                                break;
                            case DaoExpression.LtEq:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.LessThanOrEqual,
                                                     param.Value);
                                break;
                            case DaoExpression.NotEq:
                                result = ApplyFilter(result, param.Property, ExtendedLinqExpressionType.NotEqual,
                                                     param.Value);
                                break;
                        }
                }
            }
            if (sortParam != null)
            {
                result = sortParam.Ascending
                             ? result.OrderBy(sortParam.Property)
                             : result.OrderByDescending(sortParam.Property);
            }
            else
            {
                result = result.OrderBy("Id");
            }
            return result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }
        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            var orderedType = type;
            foreach (var pi in props.Select(orderedType.GetProperty))
            {
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        static IQueryable<T> ApplyFilter<T>(IQueryable<T> source, string property, ExtendedLinqExpressionType expressionType, object paramValue)
        {
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            expr = Expression.Property(expr, property);
            Expression expression = null;
            if (Enum.IsDefined(typeof(ExpressionType), (int)expressionType))
            {
                ConstantExpression constValue = Expression.Constant(paramValue, paramValue.GetType());
                expression = Expression.MakeBinary((ExpressionType)expressionType, expr, constValue);
            }
            else
            {
                try
                {
                    expression = Expression.Call(Expression.PropertyOrField(arg, property),
                                                 typeof(string).GetMethod(expressionType.ToString(), new[] { typeof(string) }),
                                                 new Expression[] { Expression.Constant(paramValue, typeof(string)) });
                }
                catch (Exception exception)
                {
                    throw new OperationCanceledException("There is no implementation of following filter type",
                                                         exception);
                }
            }
            var lambda = Expression.Lambda<Func<T, bool>>(expression, new[] { arg });
            var result = source.Where(lambda);

            return result;
        }

        internal static object GetPropertyValue(object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).GetValue(instance, BindingFlags.GetProperty, null, null, CultureInfo.CurrentCulture);
        }

        internal static bool Contains(string a, string b)
        {
            return SqlMethods.Like(a, b);// a.Contains(b);
        }

        internal static bool EndsWith(string a, string b)
        {
            return a.EndsWith(b);
        }

        internal static bool StartsWith(string a, string b)
        {
            return a.StartsWith(b);
        }
    }
}
