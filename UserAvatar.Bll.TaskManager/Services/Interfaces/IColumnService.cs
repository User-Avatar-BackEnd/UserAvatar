using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;
//using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IColumnService
    {
        Task<ColumnModel> CreateAsync(int userId, int boardId, string title);

        Task ChangePositionAsync(int userId, int columnId, int positionIndex);

        Task UpdateAsync(int userId, int columnId, string title);

        Task DeleteAsync(int userId, int columnId);

        Task<ColumnModel> GetColumnByIdAsync(int userId, int columnId);

        Task<List<ColumnModel>> GetAllColumnsAsync(int userId, int boardId);
    }
}