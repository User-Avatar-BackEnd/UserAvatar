using System;
using UserAvatar.DAL.Entities;

namespace UserAvatar.BLL.DTOs
{
    public class UserDto
    {
        public UserDto(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Login = user.Login;
            PasswordHash = user.PasswordHash;
            Score = user.Score;
            Role = user.Role;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int Score { get; set; }
        public string Role { get; set; }
    }
}
