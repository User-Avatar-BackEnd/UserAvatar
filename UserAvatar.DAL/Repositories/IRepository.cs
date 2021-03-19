using System;
using System.Collections.Generic;

namespace UserAvatar.DAL.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public T Get(int id);
        public void Create(T item);
        public void Update(T item);
        public void Delete(int id);
    }
}
