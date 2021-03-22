using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<User> GetByEmail(string email)
        {
            return await _dbContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _dbContext.Users
                .Include(user=> user.Invited)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task Create(User user)
        {
           await _dbContext.Users.AddAsync(user);
           await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsLoginExist(string login)
        {
            return await _dbContext.Users.AnyAsync(user => user.Login == login);
        }

        public async Task<bool> IsUserExist(string email)
        {
            return await _dbContext.Users.AnyAsync(user => user.Email == email);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
}