using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models;

namespace MerryClosets.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(long id);

        T GetByReference(string reference);

        List<T> List();

        List<T> List(ISpecification<T> spec);

        T Add(T entity);

        T Update(T entity);

        T Delete(T entity);
    }
}