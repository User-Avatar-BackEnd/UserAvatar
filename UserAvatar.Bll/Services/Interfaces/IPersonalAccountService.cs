namespace UserAvatar.Bll.Services.Interfaces
{
    public interface IPersonalAccountService
    {
        void ChangePassword(int userId, string oldPassword, string newPassword);
        void ChangeLogin(int userId, string newLogin);

    }
}