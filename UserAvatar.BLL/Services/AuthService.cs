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
        private readonly IMapper _mapper;

        public AuthService(UserStorage userStorage)
        {
            _userStorage = userStorage;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>()).CreateMapper();
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

            if (_userStorage.IsUserExist(email)) return null;

            _userStorage.Create(user);
            
            return _mapper.Map<User, UserDto>(user);
        }

        public UserDto Login(string email, string password)
        {
            var user = _userStorage.GetByEmail(email);

            if (user == null) return null;
            
            return !PasswordHash.ValidatePassword(password, user.PasswordHash) ? null : _mapper.Map<User, UserDto>(user);
            
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
