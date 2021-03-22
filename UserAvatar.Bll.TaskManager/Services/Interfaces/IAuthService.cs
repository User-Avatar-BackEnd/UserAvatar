using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IAuthService
    {
        public UserModel Register(string email, string login, string password);

        public UserModel Login(string email, string password);
    }
}