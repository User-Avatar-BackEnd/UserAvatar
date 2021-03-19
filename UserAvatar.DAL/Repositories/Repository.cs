using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;

namespace UserAvatar.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly UserAvatarContext _dbContext;

        public Repository(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(T item)
        {
            _dbContext.Set<T>().Add(item);
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Set<T>().Find(id);
            if (entity != null)
                _dbContext.Set<T>().Remove(entity);
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public T Get(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public void Update(T item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }

    }
}
