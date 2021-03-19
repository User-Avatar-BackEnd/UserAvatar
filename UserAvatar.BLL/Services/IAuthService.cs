using System.Collections.Generic;
using UserAvatar.BLL.DTOs;

namespace UserAvatar.BLL.Services
{
    public interface IAuthService
    {
        int Register(string email, string password);
        List<UserDto> GetALlUsers();
        
    }
}