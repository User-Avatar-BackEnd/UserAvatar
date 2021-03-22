using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface IColumnStorage
    {
        Task<Column> Create(Column column);
        Task DeleteApparent(int columnId);
        Task Update(Column column);
        Task ChangePosition(int columnId, int positionIndex);
        Task<Column> GetColumnById(int id);
        Task RecurrentlyDelete(IEnumerable<Column> columns);
        Task<IQueryable<Column>> GetAllColumns(int boardId);
        bool IsUserInBoardByColumnId(int userId, int columnId);

    }
}