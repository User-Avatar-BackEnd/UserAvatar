using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User> GetByEmailAsync(string email)
        {
            //return await _dbContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            
            return await _dbContext.Users
                .Where(user => user.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> FindByQuery(string query)
        {
            return await _dbContext.Users.Where(x => x.Login.Contains(query)).Take(10).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(user=> user.Invited)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task CreateAsync(User user)
        {
           await _dbContext.Users.AddAsync(user);
           await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsLoginExistAsync(string login)
        {
            return await _dbContext.Users
                .AnyAsync(user => user.Login == login);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task<bool> IsUserExistAsync(string email)
        {
            //return await _dbContext.Users.AnyAsync(user => user.Email == email);
            return await _dbContext.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }
    }
}