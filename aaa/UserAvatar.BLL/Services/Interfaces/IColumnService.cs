using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.BLL.Models;
using UserAvatar.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.BLL.Services.Interfaces
{
    public interface IColumnService
    {
        Task<ColumnModel> Create(int userId, int boardId, string title);
        Task ChangePosition(int userId, int columnId, int positionIndex);
        Task Update(int userId, int columnId, string title);
        Task Delete(int userId, int columnId);
        Task<ColumnModel> GetColumnById(int userId, int columnId);

        Task<List<ColumnModel>> GetAllColumns(int userId, int boardId);
    }
}