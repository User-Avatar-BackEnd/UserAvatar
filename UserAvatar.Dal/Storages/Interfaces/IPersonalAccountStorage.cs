using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IPersonalAccountStorage
    {
        void Update(User user);
    }
}