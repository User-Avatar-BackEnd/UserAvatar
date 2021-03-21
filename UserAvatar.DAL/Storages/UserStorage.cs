using System;
using System.Linq;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
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

        public bool IsUserExist(string email)
        {
            return _dbContext.Set<User>().Any(user => user.Email == email);
        }
    }
}