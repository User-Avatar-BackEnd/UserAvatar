using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IUserStorage
    {
        public Task<User> GetByEmailAsync(string email);

        public Task CreateAsync(User user);

        public Task<bool> IsLoginExistAsync(string login);

        public Task<bool> IsUserExistAsync(string email);

        public Task<User> GetByIdAsync(int id);
    }
}