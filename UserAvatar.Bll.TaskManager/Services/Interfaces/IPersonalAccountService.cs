using System.Threading.Tasks;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IPersonalAccountService
    {
        Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task ChangeLoginAsync(int userId, string newLogin);

    }
}