using System.Collections.Generic;
using UserAvatar.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IColumnStorage
    {
        Task Create(Column column);
        Task DeleteApparent(int columnId);
        Task Update(Column column);
        Task ChangePosition(int columnId, int positionIndex);
        Column GetColumnById(int id);
        Task RecurrentlyDelete(IEnumerable<Column> columns);
    }
}