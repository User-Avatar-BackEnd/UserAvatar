using UserAvatar.Bll.Models;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface ITaskService
    {
        public TaskModel GetById(int taskId, int userId);
    }
}
