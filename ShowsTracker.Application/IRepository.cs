using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application
{
    public interface IRepository<Table> where Table : IBaseEntity
    {
        IQueryable<Table> GetAll();

        Task<Table> GetById(Guid id);

        Task DeleteById(Guid id);

        Task Delete(Table entity);

        Task Update(Table entity);

        Task<Table> Create(Table entity);

    }
}
