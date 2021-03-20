using AutoMapper;
using System;
using UserAvatar.BLL.Models;
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
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserModel>()).CreateMapper();
        }

        public UserModel Register(string email, string password)
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
            
            return _mapper.Map<User, UserModel>(user);
        }

        public UserModel Login(string email, string password)
        {
            var user = _userStorage.GetByEmail(email);

            if (user == null) return null;
            
            return !PasswordHash.ValidatePassword(password, user.PasswordHash) ? null : _mapper.Map<User, UserModel>(user);
            
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
