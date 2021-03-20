using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using UserAvatar.BLL.DTOs;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages;

namespace UserAvatar.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserStorage _userStorage;

        public AuthService(UserStorage userStorage)
        {
            _userStorage = userStorage;
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
            _userStorage.Create(user);
            return user.Id;
        }

        public UserDto GetUserByEmail(string email)
        {
            var user = _userStorage.GetByEmail(email);
            if (user == null) return null;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>()).CreateMapper();
            return mapper.Map<User, UserDto>(user);
        }
    }
}
