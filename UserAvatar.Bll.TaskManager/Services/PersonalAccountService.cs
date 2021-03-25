using AutoMapper;
using System;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;
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


        public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userStorage.GetByIdAsync(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (!PasswordHash.ValidatePassword(oldPassword, user.PasswordHash)) throw new SystemException("Invalid old password");

            if (PasswordHash.ValidatePassword(newPassword,user.PasswordHash)) throw new SystemException("New password can't be the same as the old password");

            user.PasswordHash = PasswordHash.CreateHash(newPassword);

            await _userStorage.UpdateAsync(user);
        }

        public async Task ChangeLoginAsync(int userId, string newLogin)
        {
            var user = await _userStorage.GetByIdAsync(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (user.Login == newLogin) throw new SystemException("This is current login");

            if (await _userStorage.IsLoginExistAsync(newLogin)) throw new SystemException("This login is already taken");

            user.Login = newLogin;

            await _userStorage.UpdateAsync(user);
        }

        public async Task ChangeRole(int userId, int editingUserId, string role)
        {
            //VALIDATIONC _> THAT HE IS ADMIN AMD USERID != EDITING

            var user = await _userStorage.GetByIdAsync(editingUserId);

            user.Role = role;

            // validations and calling the storage
            await _userStorage.UpdateAsync(user);

            throw new NotImplementedException();
        }

        public async Task<UserModel> GetUsersDataAsync(int userId)
        {
            var user = await _userStorage.GetByIdAsync(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            return _mapper.Map<User, UserModel>(user);
        }
    }
}