using System;
using System.Linq;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.DAL.Storages
{
    public class UserStorage : IUserStorage
    {
        private readonly UserAvatarContext _dbContext;

        public UserStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public User GetByEmail(string email)
        {
            return _dbContext.Set<User>().Where(user => user.Email == email).FirstOrDefault();
        }

        public User GetById(int id)
        {
            return _dbContext.Set<User>().FirstOrDefault(x => x.Id == id);
        }

        public void Create(User user)
        {
            _dbContext.Set<User>().Add(user);
            _dbContext.SaveChanges();
        }

        public bool IsLoginExist(string login)
        {
            return _dbContext.Set<User>().Any(user => user.Login == login);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
