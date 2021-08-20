using System.Collections.Generic;
using VacationRental.DAL.Model;

namespace VacationRental.DAL.Repositories
{
    public interface IGenericRepository<TKey, TEntity> where TEntity : IIdentifier<TKey>
    {
        TEntity Load(TKey id);
        TKey Add(TEntity rental);
        IEnumerable<TEntity> LoadAll();
    }
}