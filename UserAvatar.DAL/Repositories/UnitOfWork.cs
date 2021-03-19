using System;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Repositories
{
    public class UnitOfWork : IDisposable,IUnitOfWork
    {
        private readonly UserAvatarContext _db;

        public UnitOfWork(UserAvatarContext db)
        {
            _db = db;
        }

        private Repository<User> _userRepository;

        public Repository<User> Users
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new Repository<User>(_db);
                return _userRepository;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
 }
