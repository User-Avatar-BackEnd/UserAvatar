using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Bll.Models;
using UserAvatar.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface IColumnService
    {
        Task<ColumnModel> Create(int boardId, string title);
        Task ChangePosition(int columnId, int positionIndex);
        Task Update(int columnId, string title);
        Task Delete(int columnId);
        Task<ColumnModel> GetColumnById(int columnId);

        Task<List<ColumnModel>> GetAllColumns(int boardId);
    }
}