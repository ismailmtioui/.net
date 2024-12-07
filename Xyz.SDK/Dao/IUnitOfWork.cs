namespace Xyz.SDK.Dao
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
