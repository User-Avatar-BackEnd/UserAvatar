using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public interface IUserStorage
    {
        User GetByEmail(string email);
        void Create(User user);
        bool IsLoginExist(string login);

        User GetById(int id);
    }
}