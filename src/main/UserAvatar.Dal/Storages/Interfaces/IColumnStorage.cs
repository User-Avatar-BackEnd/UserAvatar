using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Dal.Storages.Interfaces;

public interface IColumnStorage
{
    Task<int> CountColumnsInBoardAsync(int boardId);
    Task AddColumnAsync(Column column);
    Task<List<Column>> InternalGetAllColumnsAsync(Column column);
    Task<List<Column>> GetAllColumnsExceptThisAsync(Column thisColumn);
    Task UpdateAsync();
    Task DeleteApparentAsync(Column column);
    Task UpdateAsync(Column column);
    Task<Column> GetColumnByIdAsync(int id);
    Task<List<int>> GetAllColumnsAsync(int boardId);
    Task<int> GetColumnsCountInBoardAsync(int boardId);
}
