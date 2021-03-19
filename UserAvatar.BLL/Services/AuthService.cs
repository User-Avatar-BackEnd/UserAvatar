using System;
using System.Collections.Generic;
using System.Linq;
using UserAvatar.BLL.DTOs;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Repositories;

namespace UserAvatar.BLL.Services
{
    public class AuthService
    {
        private UnitOfWork _unitOfWork;

        public AuthService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public int Register(string email, string password)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = password.GetHashCode().ToString(),
                Login = "user31257825324",
                Score = 0,
                Role = "user"
            };
            _unitOfWork.Users.Create(user);
            return user.Id;
        }

        public List<UserDto> GetALlUsers()
        {
            return _unitOfWork.Users.GetAll().Select(x=>new UserDto(x)).ToList();
        }
    }
}
