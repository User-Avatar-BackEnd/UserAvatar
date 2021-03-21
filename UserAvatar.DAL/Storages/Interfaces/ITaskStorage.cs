using System;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface ITaskStorage
    {
        public Task GetById(int id);
    }
}
