using System.Collections.Generic;
using UserAvatar.BLL.DTOs;

namespace UserAvatar.BLL.Services
{
    public interface IAuthService
    {
        public UserDto Register(string email, string password);

        public UserDto Login(string email, string password);
    }
}