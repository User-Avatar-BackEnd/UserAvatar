using System;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public interface ITaskStorage
    {
        public Task GetById(int id);
    }
}
