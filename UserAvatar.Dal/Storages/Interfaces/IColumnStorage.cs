using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IColumnStorage
    {
        Task<Column> CreateAsync(Column column);

        Task DeleteApparentAsync(int columnId);

        Task UpdateAsync(Column column);

        Task ChangePositionAsync(int columnId, int positionIndex);

        Task<Column> GetColumnByIdAsync(int id);

        //Task RecurrentlyDeleteAsync(IEnumerable<Column> columns);

        Task<List<int>> GetAllColumnsAsync(int boardId);
        
        Task<int> GetColumnIdByBoardId(int boardId);

        bool IsUserInBoardByColumnId(int userId, int columnId);
    }
}