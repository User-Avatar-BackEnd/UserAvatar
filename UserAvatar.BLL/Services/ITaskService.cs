using System;
using UserAvatar.BLL.Models;

namespace UserAvatar.BLL.Services
{
    public interface ITaskService
    {
        public TaskModel GetById(int taskId, int userId);
    }
}
