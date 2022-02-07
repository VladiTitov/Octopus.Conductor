using Microsoft.EntityFrameworkCore;
using Octopus.Conductor.Application.Interfaces;
using Octopus.Conductor.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RelationalDB.Repositories
{
    public class EfRepository : IRepository
    {
        private readonly EntitiesDbContext _context;

        public EfRepository(EntitiesDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync<TEntity>(
            TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(
            int id,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
