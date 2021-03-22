using UserAvatar.Bll.Models;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface ITaskService
    {
        public TaskModel GetById(int taskId, int userId);

        public TaskModel CreateTask(string title, int columnId, int userId);

        public void UpdateTask(TaskModel task, int columnId, int? responsibleId, int userId);

        public void DeleteTask(int taskId, int userId);
    }
}
