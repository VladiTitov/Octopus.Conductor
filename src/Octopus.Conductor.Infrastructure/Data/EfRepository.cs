using Microsoft.EntityFrameworkCore;
using Octopus.Conductor.Core.Common;
using Octopus.Conductor.Core.Entities;
using Octopus.Conductor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.Data
{
    public class EfRepository : IRepository
    {
        private readonly AppDbContext _context;

        public EfRepository(AppDbContext context)
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
