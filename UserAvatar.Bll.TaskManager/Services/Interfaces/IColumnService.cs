using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IColumnService
    {
        Task<Result<ColumnModel>> CreateAsync(int userId, int boardId, string title);

        Task<int> ChangePositionAsync(int userId, int boardId, int columnId, int positionIndex);

        Task<int> DeleteAsync(int userId, int boardId, int columnId);

        Task<int> UpdateAsync(int userId, int boardId, int columnId, string title);

        Task<Result<ColumnModel>> GetColumnByIdAsync(int userId, int boardId, int columnId);
        
    }
}