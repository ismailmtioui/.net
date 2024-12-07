using Xyz.SDK.Domain;

namespace Xyz.SDK.Dao
{
    public interface ISupportsSave<TEntity, TPk> where TEntity : EntityBase<TPk>
    {
        Task<TEntity> InsertAsync(TEntity entity);

        void Update(TEntity entity);
    }
}
