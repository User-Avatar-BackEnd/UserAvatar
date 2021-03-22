using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.Services;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Services
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


        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userStorage.GetById(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (!PasswordHash.ValidatePassword(oldPassword, user.PasswordHash)) throw new SystemException("Invalid old password");

            var newPasswordHash = PasswordHash.CreateHash(newPassword);

            if (user.PasswordHash == newPasswordHash) throw new SystemException("New password can't be the same as the old password");

            user.PasswordHash = newPasswordHash;

             _personalAccountStorage.Update(user);
        }

        public void ChangeLogin(int userId, string newLogin)
        {
            var user = _userStorage.GetById(userId);

            if (user == null) throw new SystemException("User doesn't exist");

            if (user.Login == newLogin) throw new SystemException("This is current login");

            if (_userStorage.IsLoginExist(newLogin)) throw new SystemException("This login is already taken");

            user.Login = newLogin;

            _personalAccountStorage.Update(user);
        }

    }
}