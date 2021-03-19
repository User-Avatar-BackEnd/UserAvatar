using System.Collections.Generic;
using UserAvatar.BLL.DTOs;

namespace UserAvatar.BLL.Services
{
    public interface IAuthService
    {
        public int Register(string email, string password);
        public UserDto GetUserByEmail(string email);
    }
}