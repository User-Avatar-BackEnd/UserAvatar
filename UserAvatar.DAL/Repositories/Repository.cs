using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;

namespace UserAvatar.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected UserAvatarContext _db;

        public Repository(UserAvatarContext context)
        {
            _db = context;
        }

        public void Create(T item)
        {
            _db.Set<T>().Add(item);
        }

        public void Delete(int id)
        {
            T entity = _db.Set<T>().Find(id);
            if (entity != null)
                _db.Set<T>().Remove(entity);
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _db.Set<T>().Where(predicate);
        }

        public T Get(int id)
        {
            return _db.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public void Update(T item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

    }
}
