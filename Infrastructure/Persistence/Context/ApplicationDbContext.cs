using Application.Interfaces.Services;

using Domain.Common;
using Domain.Entity;

using Microsoft.EntityFrameworkCore;

using Persistence.Context.EntityConfigurations;

namespace Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
            _authenticatedUser = authenticatedUser ?? throw new ArgumentNullException(nameof(authenticatedUser));
        }

        public DbSet<Todo> Todos { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = _dateTime.UtcNow;
                        entry.Entity.CreatedBy = _authenticatedUser?.UserId?? string.Empty;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _authenticatedUser?.UserId ?? string.Empty;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply configurations
            builder.ApplyConfiguration(new TodoEntityConfiguration());

            // Set decimal column type
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);
        }
    }
}
