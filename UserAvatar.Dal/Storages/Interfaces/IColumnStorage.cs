using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IColumnStorage
    {
        Task<int> CountColumnsInBoardAsync(int boardId);
        Task AddColumnAsync(Column column);
        Task<List<Column>> InternalGetAllColumns(Column column);
        Task<List<Column>> GetAllColumnsExceptThis(Column thisColumn);
        Task Update();
        Task DeleteApparentAsync(Column column);
        Task UpdateAsync(Column column);
        Task<Column> GetColumnByIdAsync(int id);
        Task<List<int>> GetAllColumnsAsync(int boardId);
        Task<int> GetColumnsCountInBoardAsync(int boardId);
    }
}