using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CinemaDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(CinemaDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }
    }
}
