﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UCosmic.Domain
{
    internal static class EntityExtensions
    {
        internal static int NextNumber(this IEnumerable<IAmNumbered> enumerable)
        {
            var collection = enumerable.Select(x => x.Number).ToArray();
            var max = collection.Any() ? collection.Max() : 0;
            return ++max;
        }

        private static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> queryable,
            Expression<Func<TEntity, object>> expression,
            IEagerLoad entities)
            where TEntity : Entity
        {
            return entities.EagerLoad(queryable, expression);
        }

        internal static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> queryable,
            IEagerLoad entities,
            IEnumerable<Expression<Func<TEntity, object>>> expressions)
            where TEntity : Entity
        {
            if (expressions != null)
                queryable = expressions.Aggregate(queryable, (current, expression) => current.EagerLoad(expression, entities));
            return queryable;
        }

        internal static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> queryable,
            IEnumerable<KeyValuePair<Expression<Func<TEntity, object>>, OrderByDirection>> expressions)
        {
            // http://stackoverflow.com/a/9155222/304832
            if (expressions != null)
            {
                // first expression is passed to OrderBy/Descending, others are passed to ThenBy/Descending
                var counter = 0;
                foreach (var expression in expressions)
                {
                    var unaryExpression = expression.Key.Body as UnaryExpression;
                    var memberExpression = unaryExpression != null ? unaryExpression.Operand as MemberExpression : null;
                    var methodExpression = unaryExpression != null ? unaryExpression.Operand as MethodCallExpression : null;
                    var binaryExpression = unaryExpression != null ? unaryExpression.Operand as BinaryExpression : null;
                    var conditionalExpression = unaryExpression != null ? unaryExpression.Operand as ConditionalExpression : null;
                    var operandExpression = memberExpression ?? methodExpression ?? binaryExpression ?? conditionalExpression as Expression;

                    if (operandExpression != null)

                        if (operandExpression.Type == typeof(string))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, string>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(char))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, char>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(char?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, char?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(DateTime))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, DateTime>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(DateTime?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, DateTime?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(bool))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, bool>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(bool?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, bool?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(int))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, int>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(int?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, int?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(long))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, long>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(long?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, long?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(short))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, short>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(short?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, short?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(double))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, double>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(double?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, double?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(float))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, float>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(float?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, float?>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(decimal))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, decimal>>(operandExpression, expression.Key.Parameters));

                        else if (operandExpression.Type == typeof(decimal?))
                            queryable = queryable.ApplyOrderBy(counter, expression.Value,
                                Expression.Lambda<Func<TEntity, decimal?>>(operandExpression, expression.Key.Parameters));

                        else
                            throw new NotImplementedException(string.Format(
                                "OrderBy object type resolution is not yet implemented for '{0}'.", operandExpression.Type.Name));

                    else
                        queryable = queryable.ApplyOrderBy(counter, expression.Value, expression.Key);

                    ++counter;
                }
            }
            return queryable;
        }

        private static IQueryable<TEntity> ApplyOrderBy<TEntity, TKey>(this IQueryable<TEntity> queryable, int counter, OrderByDirection direction, Expression<Func<TEntity, TKey>> keySelector)
        {
            if (counter == 0)
            {
                queryable = direction == OrderByDirection.Ascending
                    ? queryable.OrderBy(keySelector)
                    : queryable.OrderByDescending(keySelector);
            }
            else
            {
                queryable = direction == OrderByDirection.Ascending
                    ? ((IOrderedQueryable<TEntity>)queryable).ThenBy(keySelector)
                    : ((IOrderedQueryable<TEntity>)queryable).ThenByDescending(keySelector);
            }
            return queryable;
        }


        //internal static TRevisableEntity By<TRevisableEntity>(this IEnumerable<TRevisableEntity> enumerable, Guid entityId)
        //    where TRevisableEntity : RevisableEntity
        //{
        //    return enumerable.AsQueryable().By(entityId);
        //}

        //internal static TRevisableEntity By<TRevisableEntity>(this IEnumerable<TRevisableEntity> enumerable, int revisionId)
        //    where TRevisableEntity : RevisableEntity
        //{
        //    return enumerable.AsQueryable().By(revisionId);
        //}

        //internal static TRevisableEntity By<TRevisableEntity>(this IQueryable<TRevisableEntity> queryable, Guid entityId)
        //    where TRevisableEntity : RevisableEntity
        //{
        //    if (entityId == Guid.Empty)
        //        throw new InvalidOperationException(string.Format("EntityId Guid is empty ({0}).", Guid.Empty));
        //    return queryable.SingleOrDefault(e => e.EntityId == entityId);
        //}

        //internal static TRevisableEntity By<TRevisableEntity>(this IQueryable<TRevisableEntity> queryable, int revisionId)
        //    where TRevisableEntity : RevisableEntity
        //{
        //    return queryable.SingleOrDefault(e => e.RevisionId == revisionId);
        //}

        //internal static IQueryable<TRevisableEntity> Exclude<TRevisableEntity>(this IQueryable<TRevisableEntity> queryable, IEnumerable<Guid> entityIds)
        //    where TRevisableEntity : RevisableEntity
        //{
        //    if (entityIds != null)
        //        queryable = queryable.Where(e => !entityIds.Contains(e.EntityId));
        //    return queryable;
        //}
    }
}