using System;
using UserAvatar.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface IColumnStorage
    {
        Task Create(Column column);
        void DeleteApparent(int columnId);
        void Update(Column column);
        Task ChangePosition(int columnId, int positionIndex);
        Column GetColumnById(int id);
    }
}