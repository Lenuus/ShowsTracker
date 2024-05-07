using Microsoft.EntityFrameworkCore;
using ShowsTracker.Domain;
using ShowsTracker.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application
{
    public class Repository<Table> : IRepository<Table> where Table : class, IBaseEntity
    {
        private readonly MainDbContext _context;
        private DbSet<Table> _table;

        public Repository(MainDbContext context)
        {
            _context = context;
            _table = _context.Set<Table>();
        }
        public async Task<Table> Create(Table entity)
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Table entity)
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteById(Guid id)
        {
            var query = _table.Find(id);
            if (query == null)
                throw new Exception("Not Found");

            _table.Remove(query);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Table> GetAll()
        {
            return _table;
        }

        public async Task<Table> GetById(Guid id)
        {
            var query = await _table.FindAsync(id);
            if (query == null)
                throw new Exception("Not Found");

            return query;

        }

        public async Task Update(Table entity)
        {
            _table.Update(entity);

            await _context.SaveChangesAsync();

        }
    }
}
