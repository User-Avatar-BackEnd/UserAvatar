using AutoMapper;
using System;
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

        public UserDto Register(string email, string password)
        {
            var login = GenerateLogin();

            var user = new User
            {
                Email = email,
                PasswordHash = PasswordHash.CreateHash(password),
                Login = login,
                Score = 0,
                Role = "user"
            };

            _userStorage.Create(user);

            return new UserDto(user);
        }

        public UserDto Login(string email, string password)
        {
            var user = _userStorage.GetByEmail(email);

            if (user == null) return null;

            if (user.PasswordHash != PasswordHash.CreateHash(password)) return null;

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>()).CreateMapper();

            return mapper.Map<User, UserDto>(user);
        }

        private string GenerateLogin()
        {
            while (true)
            {
                string login = "user" + RandomDigits();

                if(!_userStorage.IsLoginExist(login)) return login;
            }
        }

        private string RandomDigits()
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < 10; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
    }
}
