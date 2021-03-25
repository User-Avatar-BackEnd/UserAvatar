using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<UserModel>> RegisterAsync(string email, string login, string password);

        public Task<Result<UserModel>> LoginAsync(string email, string password);
    }
}