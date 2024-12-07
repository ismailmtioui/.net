using Xyz.SDK.Domain;

namespace Xyz.SDK.Dao
{
    public interface ISupportsDeleteDao<TEntity, TPk> where TEntity : EntityBase<TPk>
    {
        void Delete(TEntity entity);
        void SoftDelete(TEntity entity);
        void Delete(TPk key);
        void SoftDelete(TPk key);
    }
}
