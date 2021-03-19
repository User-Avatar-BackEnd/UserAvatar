using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Repositories
{
    public interface IUnitOfWork
    {
        Repository<User> Users { get; }
        void Save();
        void Dispose(bool disposing);
        void Dispose();

    }
}