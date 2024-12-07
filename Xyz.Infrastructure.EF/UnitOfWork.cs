using Microsoft.EntityFrameworkCore;
using Xyz.SDK.Dao;
using Xyz.SDK.Domain;

namespace Xyz.Infrastructure.EF;

internal class UnitOfWork : DbContext, IUnitOfWork
{
    public UnitOfWork(DbContextOptions<UnitOfWork> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UnitOfWork).Assembly);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        foreach (var syncEntityEntry in ChangeTracker.Entries<TrackedEntity<int>>())
        {
            switch (syncEntityEntry.State)
            {
                case EntityState.Added:
                    syncEntityEntry.Entity.CreatedOn = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    syncEntityEntry.Entity.UpdatedOn = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public async Task<bool> CommitAsync()
    {
        return await SaveChangesAsync() > 0;
    }
}