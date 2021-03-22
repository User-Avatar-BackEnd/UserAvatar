using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class PersonalAccountService : IPersonalAccountService
    {
        private readonly IUserStorage _userStorage;
        private readonly IMapper _mapper;

        public PersonalAccountService(IUserStorage userStorage, IMapper mapper)
        {
            _userStorage = userStorage;
            _mapper = mapper;
        }


        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userStorage.GetById(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (!PasswordHash.ValidatePassword(oldPassword, user.PasswordHash)) throw new SystemException("Invalid old password");

            if (PasswordHash.ValidatePassword(newPassword,user.PasswordHash)) throw new SystemException("New password can't be the same as the old password");

            user.PasswordHash = PasswordHash.CreateHash(newPassword);

            _userStorage.UpdateAsync(user);
        }

        public void ChangeLogin(int userId, string newLogin)
        {
            var user = _userStorage.GetById(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (user.Login == newLogin) throw new SystemException("This is current login");

            if (_userStorage.IsLoginExist(newLogin)) throw new SystemException("This login is already taken");

            user.Login = newLogin;

            _userStorage.UpdateAsync(user);
        }

        public UserModel GetUsersData(int userId)
        {
            var user = _userStorage.GetById(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            return _mapper.Map<User, UserModel>(user);
        }
    }
}