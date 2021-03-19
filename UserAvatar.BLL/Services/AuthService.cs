using System;
using System.Collections.Generic;
using System.Linq;
using UserAvatar.BLL.DTOs;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Repositories;

namespace UserAvatar.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Register(string email, string password)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = PasswordHash.CreateHash(password),
                Login = "user31257825324",
                Score = 0,
                Role = "user"
            };
            _unitOfWork.Users.Create(user);
            _unitOfWork.Save();
            return user.Id;
        }

        public List<UserDto> GetALlUsers()
        {
            return _unitOfWork.Users.GetAll().Select(x=>new UserDto(x)).ToList();
        }
    }
}
