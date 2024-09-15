using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services;

public sealed class PersonalAccountService : IPersonalAccountService
{
    private readonly IMapper _mapper;
    private readonly IUserStorage _userStorage;

    public PersonalAccountService(IUserStorage userStorage, IMapper mapper)
    {
        _userStorage = userStorage;
        _mapper = mapper;
    }

    public async Task<int> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userStorage.GetByIdAsync(userId);

        if (user == null)
        {
            return ResultCode.NotFound;
        }

        if (!PasswordHash.ValidatePassword(oldPassword, user.PasswordHash))
        {
            return ResultCode.InvalidPassword;
        }

        if (PasswordHash.ValidatePassword(newPassword, user.PasswordHash))
        {
            return ResultCode.SamePasswordAsOld;
        }

        user.PasswordHash = PasswordHash.CreateHash(newPassword);

        await _userStorage.UpdateAsync(user);

        return ResultCode.Success;
    }

    public async Task<int> ChangeLoginAsync(int userId, string newLogin)
    {
        var user = await _userStorage.GetByIdAsync(userId);

        if (user == null)
        {
            return ResultCode.NotFound;
        }

        if (user.Login == newLogin)
        {
            return ResultCode.SameLoginAsCurrent;
        }

        if (await _userStorage.IsLoginExistAsync(newLogin))
        {
            return ResultCode.LoginAlreadyExist;
        }

        user.Login = newLogin;

        await _userStorage.UpdateAsync(user);

        return ResultCode.Success;
    }

    public async Task<int> ChangeRoleAsync(int userId, string login, string role)
    {
        var userToChange = await _userStorage.GetByLoginAsync(login);

        if (userToChange == null)
        {
            return ResultCode.NotFound;
        }

        if (userToChange.Id == userId)
        {
            return ResultCode.Forbidden;
        }

        userToChange.Role = role;

        await _userStorage.UpdateAsync(userToChange);

        return ResultCode.Success;
    }

    public async Task<Result<UserModel>> GetUsersDataAsync(int userId)
    {
        var user = await _userStorage.GetByIdAsync(userId);

        if (user == null)
        {
            return new Result<UserModel>(ResultCode.NotFound);
        }

        return new Result<UserModel>(_mapper.Map<User, UserModel>(user));
    }
}
