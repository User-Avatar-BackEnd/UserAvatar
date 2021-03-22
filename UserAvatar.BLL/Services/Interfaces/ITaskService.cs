using UserAvatar.Bll.Models;
using UserAvatar.Contracts.Requests;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface ITaskService
    {
        public TaskModel GetById(int taskId, int userId);

        public TaskModel CreateTask(AddTaskRequest addTaskRequest, int userId);

        public void DeleteTask(int taskId, int userId);
    }
}
