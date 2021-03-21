using UserAvatar.BLL.Models;

namespace UserAvatar.BLL.Services.Interfaces
{
    public interface ITaskService
    {
        public TaskModel GetById(int taskId, int userId);
    }
}
