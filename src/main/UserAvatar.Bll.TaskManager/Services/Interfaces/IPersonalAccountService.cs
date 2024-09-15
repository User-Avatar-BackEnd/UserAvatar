using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces;

public interface IPersonalAccountService
{
    Task<int> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    Task<int> ChangeLoginAsync(int userId, string newLogin);

    Task<Result<UserModel>> GetUsersDataAsync(int userId);

    Task<int> ChangeRoleAsync(int userId, string login, string role);
}
