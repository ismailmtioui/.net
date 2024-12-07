using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xyz.SDK.Dao;
using Xyz.SDK.Domain;

namespace Xyz.SDK.Service.Impl
{
    public class GenericService<TEntity, TPk> : ServiceBase, IGenericService<TEntity, TPk>
        where TEntity : EntityBase<TPk>, new()
    {
        private readonly IRepository<TEntity, TPk> _repository;

        public GenericService(IUnitOfWork uow, ILogger<GenericService<TEntity, TPk>> logger, IRepository<TEntity, TPk> repository): base(uow, logger)
        {
            _repository = repository;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var createdEntity = await _repository.InsertAsync(entity);
            if(await Uow.CommitAsync())
                Logger.Log(LogLevel.Information, $"New {typeof(TEntity).Name} added: {createdEntity}!");
            return createdEntity;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            _repository.Update(entity);
            try
            {
                await Uow.CommitAsync();
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, $"{typeof(TEntity).Name}#{entity.Id} update error! ");
                return false;
            }
            Logger.Log(LogLevel.Information, $"{typeof(TEntity).Name} updated, new: {entity}!");
            return true;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            entity.Deleted = true;
            _repository.Update(entity);
            try
            {
                await Uow.CommitAsync();
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, $"{typeof(TEntity).Name}#{entity.Id} update error! ");
                return false;
            }

            Logger.Log(LogLevel.Information, $"{typeof(TEntity).Name}#{entity.Id} Deleted!");
            return true;
        }

        public async Task<TEntity?> FindByIdAsync(TPk id)
        {
            var entity = await _repository.GetAsync(id);

            if (entity == null)
            {
                Logger.Log(LogLevel.Information, $"{typeof(TEntity).Name}#{id} Not Found!");
                return null;
            }

            return entity;
        }

        public async Task<List<TEntity>> FindAllAsync(params Expression<Func<TEntity, object>>[] dependencies)
        {
            var query = _repository.GetAll(dependencies);
            return await query.ToListAsync();
        }
    }
}
