using Microsoft.EntityFrameworkCore;

namespace awisk.common.Data.Db.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Applies a global query filter to all entities inheriting from BaseEntity
        /// so soft-deleted rows are automatically excluded from all queries.
        /// </summary>
        public static ModelBuilder ApplySoftDeleteFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity<object>).IsAssignableFrom(entityType.ClrType))
                    continue;

                var method = typeof(ModelBuilderExtensions)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, [modelBuilder]);
            }

            return modelBuilder;
        }

        private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity<object> =>
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
}
