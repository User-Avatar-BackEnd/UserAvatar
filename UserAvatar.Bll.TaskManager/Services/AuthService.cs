using AutoMapper;
using System;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
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

        public async Task<Result<UserModel>> RegisterAsync(string email, string login, string password)
        {
            if (await _userStorage.IsUserExistAsync(email))
            {
                return new Result<UserModel>(ResultCode.EmailAlreadyExist);
            }

            if (!string.IsNullOrWhiteSpace(login))
            {
                var isLoginTaken = await _userStorage.IsLoginExistAsync(login);
                if (isLoginTaken)
                {
                    return new Result<UserModel>(ResultCode.LoginAlreadyExist);
                }
            }
            else
            {
                login = await GenerateLoginAsync();
            }

            var user = new User
            {
                Email = email,
                PasswordHash = PasswordHash.CreateHash(password),
                Login = login,
                Score = 0,
                Role = "user"
            };

            await _userStorage.CreateAsync(user);
            
            var userModel = _mapper.Map<User, UserModel>(user);

            return new Result<UserModel>(userModel);
        }

        public async Task<Result<UserModel>> LoginAsync(string email, string password)
        {
            var user = await _userStorage.GetByEmailAsync(email);

            if (user == null)
            {
                return new Result<UserModel>(ResultCode.InvalidEmail);
            }

            var passwordIsValid = PasswordHash.ValidatePassword(password, user.PasswordHash);
            if (!passwordIsValid)
            {
                return new Result<UserModel>(ResultCode.InvalidPassword);
            }

            return new Result<UserModel>(_mapper.Map<User, UserModel>(user));
        }

        private async Task<string> GenerateLoginAsync()
        {
            while (true)
            {
                string login = "user" + RandomDigits();

                if(! await _userStorage.IsLoginExistAsync(login)) return login;
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
