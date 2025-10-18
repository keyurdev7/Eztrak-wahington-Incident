using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetInterface(typeof(TInterface).Name) != null)
                {
                    var newParam = Expression.Parameter(entityType.ClrType);
                    var newbody = ReplacingExpressionVisitor.
                        Replace(expression.Parameters.Single(), newParam, expression.Body);
                    modelBuilder.Entity(entityType.ClrType).
                        HasQueryFilter(Expression.Lambda(newbody, newParam));
                }
            }
        }
        public static void ApplyGlobalFilters<T>(this ModelBuilder modelBuilder, string propertyName, T value)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var foundProperty = entityType.FindProperty(propertyName);
                if (foundProperty != null && foundProperty.ClrType == typeof(T))
                {
                    var newParam = Expression.Parameter(entityType.ClrType);
                    var filter = Expression.
                        Lambda(Expression.Equal(Expression.Property(newParam, propertyName),
                        Expression.Constant(value)), newParam);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
