using System;
using System.Collections.Generic;
using System.Linq;

using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.DAL.InMemory.Repositories
{
    public class InMemoryRepository<TEntity> : IGenericRepository<int, TEntity> where TEntity : IIdentifier<int>
    {
        protected readonly IDictionary<int, TEntity> _storage = new Dictionary<int, TEntity>();
        
        public int Add(TEntity entity)
        {
            entity.Id = GenerateNextId();
            _storage[entity.Id] = entity;
            return entity.Id;
        }

        public bool Exists(int id)
        {
            return _storage.ContainsKey(id);
        }

        public TEntity LoadOrNull(int id)
        {
            if (_storage.TryGetValue(id, out var entity)) return entity;
            
            return default;
        }

        public IEnumerable<TEntity> LoadAll()
        {
            return _storage.Values;
        }

        private int GenerateNextId() => _storage.Count() + 1;
        
    }
}