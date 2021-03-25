using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IUserStorage
    {
        Task<User> GetByEmailAsync(string email);

        Task CreateAsync(User user);

        Task<bool> IsLoginExistAsync(string login);

        Task<List<User>> InviteByQuery(int boardId, string query);

        Task<bool> IsUserExistAsync(string email);

        Task<User> GetByIdAsync(int id);

        Task UpdateStatus(User user);

        Task UpdateAsync(User user);
        
        Task<List<User>> GetUsersRate();
      
        Task<User> GetByLoginAsync(string login);

        Task AddScoreToUser(int userId, int score);

        Task<List<User>> GetPagedUsersAsync(int pageNumber, int pageSize);

        Task<int> GetUsersAmount();
    }
}