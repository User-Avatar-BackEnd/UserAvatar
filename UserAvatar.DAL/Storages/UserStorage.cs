using System;
using System.Linq;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public class UserStorage
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

        public void Create(User user)
        {
            _dbContext.Set<User>().Add(user);
            _dbContext.SaveChanges();
        }
    }
}
