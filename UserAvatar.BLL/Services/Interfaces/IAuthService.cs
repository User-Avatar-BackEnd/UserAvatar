using System.Collections.Generic;
using UserAvatar.BLL.Models;

namespace UserAvatar.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        public UserModel Register(string email, string password);

        public UserModel Login(string email, string password);
    }
}