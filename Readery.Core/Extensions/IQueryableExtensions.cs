using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Readery.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(
           this IQueryable<T> source,
           bool condition,
           Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> query,
     string columnName, string sortDirection, string? navigationProperty = null)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return query;

            var entityType = typeof(T);

            // Create a parameter expression for the entity (e.g., x => x)
            var parameter = Expression.Parameter(entityType, "x");
            Expression propertyAccess;

            if (!string.IsNullOrEmpty(navigationProperty))
            {
                // Handle navigation property
                var navigationProperties = navigationProperty.Split('.');
                propertyAccess = parameter;
                foreach (var property in navigationProperties)
                {
                    var entityProperty = entityType.GetProperty(property);
                    if (entityProperty == null)
                    {
                        throw new ArgumentException($"Navigation property {property} does not exist on type {entityType.Name}");
                    }

                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, entityProperty);
                    entityType = entityProperty.PropertyType; // Move to the navigation property type
                }

                // Now access the desired column inside the navigation property
                var columnProperty = entityType.GetProperty(columnName);
                if (columnProperty == null)
                {
                    throw new ArgumentException($"Column {columnName} does not exist on type {entityType.Name}");
                }
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, columnProperty);
                entityType = columnProperty.PropertyType; // Update entityType to the column type
            }
            else
            {
                // Fallback to regular property
                var columnProperty = entityType.GetProperty(columnName);
                if (columnProperty == null)
                {
                    throw new ArgumentException($"Column {columnName} does not exist on type {entityType.Name}");
                }
                propertyAccess = Expression.MakeMemberAccess(parameter, columnProperty);
                entityType = columnProperty.PropertyType; // Update entityType to the column type
            }

            // Create the lambda expression for the property access
            var lambda = Expression.Lambda(propertyAccess, parameter);

            // Determine whether to sort ascending or descending
            var methodName = sortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

            // Call the appropriate Queryable.OrderBy/OrderByDescending method
            var orderByExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), entityType },
                query.Expression,
                Expression.Quote(lambda)
            );

            // Apply the ordering and return the query
            return query.Provider.CreateQuery<T>(orderByExpression);
        }

        /// <summary>
        ///     Specifies related entities to include in the query result.
        /// </summary>
        /// <typeparam name="T">The type of entity being queried.</typeparam>
        /// <param name="source">The source <see cref="IQueryable{T}" /> on which to call Include.</param>
        /// <param name="paths">The lambda expressions representing the paths to include.</param>
        /// <returns>A new <see cref="IQueryable{T}" /> with the defined query path.</returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] paths)
        {
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    source = paths.Aggregate(source, (current, include) => current.Include(include));
                }
            }

            return source;
        }
    }
}

