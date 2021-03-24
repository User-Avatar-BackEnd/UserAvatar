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

        Task<bool> IsUserExistAsync(string email);

        Task<User> GetByIdAsync(int id);

        Task UpdateAsync(User user);

        Task<List<User>> GetUsersRate();
    }
}