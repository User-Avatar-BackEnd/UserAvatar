using System;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ITaskStorage
    {
        public Task GetById(int id);
    }
}
