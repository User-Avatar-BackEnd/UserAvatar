using System;
using UserAvatar.DAL.Entities;

namespace UserAvatar.BLL.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int Score { get; set; }
        public string Role { get; set; }
    }
}
