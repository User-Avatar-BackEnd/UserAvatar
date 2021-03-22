using AutoMapper;
using System;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;
using UserAvatar.Infrastructure.Exceptions;

namespace UserAvatar.Bll.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserStorage _userStorage;
        private readonly IMapper _mapper;

        public AuthService(IUserStorage userStorage, IMapper mapper)
        {
            _userStorage = userStorage;
            _mapper = mapper;
        }

        public UserModel Register(string email, string login, string password)
        {
            if (_userStorage.IsUserExist(email))
            {
                throw new InformException("User with such email already exist");
            }

            if (login != null)
            {
                login = login.Trim();
                if (login.Length == 0)
                {
                    login = null;
                }
                else
                {
                    //Todo: config file
                    if (login.Length < 5 || login.Length > 64)
                    {
                        throw new InformException("Login must have minimum 5 symbols and maximum 64 symbols");
                    }

                    var isLoginTaken = _userStorage.IsLoginExist(login);
                    if (isLoginTaken)
                    {
                        throw new InformException("User with such login already exist");
                    }
                }
            }

            if (login == null)
            {
                login = GenerateLogin();
            }

            var user = new User
            {
                Email = email,
                PasswordHash = PasswordHash.CreateHash(password),
                Login = login,
                Score = 0,
                Role = "user"
            };

            _userStorage.Create(user);
            
            return _mapper.Map<User, UserModel>(user);
        }

        public UserModel Login(string email, string password)
        {
            var user = _userStorage.GetByEmail(email);

            if (user == null)
            {
                throw new InformException("User with this email does not exist");
            }

            var passwordIsValid = PasswordHash.ValidatePassword(password, user.PasswordHash);
            if (!passwordIsValid)
            {
                throw new InformException("Wrong password");
            }

            return _mapper.Map<User, UserModel>(user);
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
