using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<UserModel>> RegisterAsync(string email, string login, string password);

        Task<Result<UserModel>> LoginAsync(string email, string password);

        string Logout();
    }
}