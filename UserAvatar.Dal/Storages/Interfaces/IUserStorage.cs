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

        Task<List<User>> InviteByQueryAsync(int boardId, string query);

        Task<bool> IsUserExistAsync(string email);

        Task<User> GetByIdAsync(int id);

        Task UpdateStatusAsync(User user);

        Task UpdateAsync(User user);
        
        Task<List<User>> GetUsersRateAsync();
      
        Task<User> GetByLoginAsync(string login);

        Task AddScoreToUserAsync(int userId, int score);

        Task<List<User>> GetPagedUsersAsync(int pageNumber, int pageSize, string query);

        Task<int> GetUsersAmountAsync();
    }
}