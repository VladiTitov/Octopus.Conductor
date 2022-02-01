using Octopus.Conductor.Core.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Core.Interfaces
{
    public interface IRepository
    {
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;
        Task<TEntity> AddAsync<TEntity>(
            TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;
        Task<TEntity> GetByIdAsync<TEntity>(
            int id,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;
        void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
