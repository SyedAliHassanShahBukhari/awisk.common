using awisk.common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace awisk.common.Data.Db.Interceptors
{
    public class AuditInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, ct);
        }

        private void ApplyAudit(DbContext? context)
        {
            if (context is null) return;

            var userId = currentUserService.UserId ?? "system";
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity<object>>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = now;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedOn = now;
                }
            }
        }
    }
}
