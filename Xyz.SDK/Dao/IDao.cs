using System.Linq.Expressions;
using Xyz.SDK.Domain;

namespace Xyz.SDK.Dao
{
    public interface IDao<TEntity, TPk> where TEntity : EntityBase<TPk>
    {
        Task<TEntity?> GetAsync(TPk id);

        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);
    }
}
