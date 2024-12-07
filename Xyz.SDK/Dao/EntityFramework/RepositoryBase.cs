using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Xyz.SDK.Domain;

namespace Xyz.SDK.Dao.EntityFramework
{
    public class RepositoryBase<TDbContext, TEntity, TPk> : IRepository<TEntity, TPk> 
        where TEntity : EntityBase<TPk>, new() 
        where TDbContext : DbContext
    {
        protected readonly TDbContext Context;
        protected readonly DbSet<TEntity> Set;
        protected RepositoryBase(TDbContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(TPk id)
        {
            var entity = await Set.FindAsync(id);
            return entity?.Deleted == true ? null : entity;
        }

        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> entities = Set;
            foreach (var include in includes)
                entities = entities.Include(include);
            
            return entities.AsNoTracking().Where(e => e.Deleted != true);
        }

        public void Delete(TEntity entity)
        {
            Set.Remove(entity);
        }

        public void SoftDelete(TEntity entity)
        {
            entity.Deleted = true;
            Update(entity);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await Set.AddAsync(entity);
            return result.Entity;
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TPk key)
        {
            var entity = new TEntity { Id = key };
            Set.Attach(entity);
            Set.Remove(entity);
        }

        public void SoftDelete(TPk key)
        {
            var entity = new TEntity { Id = key };
            Set.Attach(entity);
            SoftDelete(entity);
        }

        public void DetachEntity(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }
    }
}
