using System;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ITaskStorage
    {
        public Task GetById(int id);

        public Task Create(Task task);

        public int GetTasksCountInColumn(int columnId);

        public int GetBoardId(int taskId);

        public void Delete(int taskId);

        public void Update(Task task);
    }
}
