using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IUserStorage
    {
        User GetByEmail(string email);
        void Create(User user);
        bool IsLoginExist(string login);
        bool IsUserExist(string email);
        User GetById(int id);
    }
}