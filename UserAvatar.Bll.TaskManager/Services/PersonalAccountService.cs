using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class PersonalAccountService : IPersonalAccountService
    {
        private readonly IUserStorage _userStorage;
        // There is just to use Update
        private readonly IPersonalAccountStorage _personalAccountStorage;
        private readonly IMapper _mapper;

        public PersonalAccountService(IUserStorage userStorage, IPersonalAccountStorage personalAccountStorage, IMapper mapper)
        {
            _userStorage = userStorage;
            _personalAccountStorage = personalAccountStorage;
            _mapper = mapper;
        }


        public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userStorage.GetByIdAsync(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (!PasswordHash.ValidatePassword(oldPassword, user.PasswordHash)) throw new SystemException("Invalid old password");

            var newPasswordHash = PasswordHash.CreateHash(newPassword);

            if (user.PasswordHash == newPasswordHash) throw new SystemException("New password can't be the same as the old password");

            user.PasswordHash = newPasswordHash;

             _personalAccountStorage.Update(user);
        }

        public async Task ChangeLoginAsync(int userId, string newLogin)
        {
            var user = await _userStorage.GetByIdAsync(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (user.Login == newLogin) throw new SystemException("This is current login");

            if (await _userStorage.IsLoginExistAsync(newLogin)) throw new SystemException("This login is already taken");

            user.Login = newLogin;

            _personalAccountStorage.Update(user);
        }

    }
}