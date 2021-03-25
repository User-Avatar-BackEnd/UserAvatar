using System;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IHistoryStorage
    {
        Task AddHstory(History history);
    }
}
