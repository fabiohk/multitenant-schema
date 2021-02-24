using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiTenantSchema.Models;

namespace MultiTenantSchema.Contexts
{
    public class MultiTenantDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("(newid())");
        }

        public override int SaveChanges()
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AutomaticallyAddCreatedAndUpdatedAt()
        {
            var entitiesOnDbContext = ChangeTracker.Entries<StandardEntity>();

            if (entitiesOnDbContext == null)
                return;

            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Added))
            {
                item.Entity.CreatedAt = System.DateTime.Now;
                item.Entity.UpdatedAt = System.DateTime.Now;
            }

            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Modified))
            {
                item.Entity.UpdatedAt = System.DateTime.Now;
            }
        }
    }
}