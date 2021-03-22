using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IUserStorage
    {
        Task<User> GetByEmail(string email);

        Task<User> GetById(int id);

        Task Create(User user);

        Task<bool> IsLoginExist(string login);

        Task<bool> IsUserExist(string email);

        Task UpdateAsync(User user);
    }
}