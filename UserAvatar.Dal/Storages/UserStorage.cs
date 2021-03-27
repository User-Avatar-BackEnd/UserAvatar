using System.Collections.Generic;
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
            return await _dbContext.Users
                .Where(user => user.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await _dbContext.Users
                .Where(user => user.Login == login)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> InviteByQuery(int boardId, string query)
        {
            var boardMembers = await  _dbContext.Members
                .Where(x => x.BoardId == boardId).Select(x=> x.User.Id).ToListAsync();

            var filtered = await _dbContext.Users
                .Where(x => !boardMembers.Contains(x.Id) 
                            && x.Login.Contains(query)).Take(10).ToListAsync();
            return filtered;
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

        public async Task UpdateStatus(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
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

        public async Task<List<User>> GetUsersRate()
        {
            return await _dbContext.Users
                .OrderByDescending(x => x.Score)
                .ToListAsync();
        }

        public async Task<bool> IsUserExistAsync(string email)
        {
            //return await _dbContext.Users.AnyAsync(user => user.Email == email);
            return await _dbContext.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }

        public async Task AddScoreToUser(int userId, int score)
        {
            var thisUser = await GetByIdAsync(userId);
            thisUser.Score += score;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetPagedUsersAsync(int pageNumber, int pageSize, string query)
        {
            // changes here
            return await _dbContext.Users
                .Where(x=> x.Login.Contains(query))
                .OrderBy(x => x.Login)
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToListAsync();
        }

        public async Task<int> GetUsersAmount()
        {
            return await _dbContext.Users.CountAsync();
        }
    }
}