using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IPersonalAccountService
    {
        void ChangePassword(int userId, string oldPassword, string newPassword);

        void ChangeLogin(int userId, string newLogin);

        UserModel GetUsersData(int userId);
    }
}