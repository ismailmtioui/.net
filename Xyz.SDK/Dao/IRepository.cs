using Xyz.SDK.Domain;

namespace Xyz.SDK.Dao
{
    public interface IRepository<TEntity, TPk> : IDao<TEntity, TPk>, ISupportsDeleteDao<TEntity, TPk>, ISupportsSave<TEntity, TPk> where TEntity : EntityBase<TPk>
    {
        void DetachEntity(TEntity entity);
    }
}
