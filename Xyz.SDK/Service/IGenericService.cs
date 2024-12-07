using System.Linq.Expressions;
using Xyz.SDK.Domain;

namespace Xyz.SDK.Service
{
    public interface IGenericService<TEntity, TPk>
        where TEntity : EntityBase<TPk>
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);

        Task<TEntity?> FindByIdAsync(TPk id);

        Task<List<TEntity>> FindAllAsync(params Expression<Func<TEntity, object>>[] dependencies);
    }
}
