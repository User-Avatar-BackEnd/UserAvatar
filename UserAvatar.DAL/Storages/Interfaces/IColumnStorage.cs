using System;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface IColumnStorage
    {
        void Create(Column column);
        void DeleteApparent(int columnId);
        void Update(Column column);
        void ChangePosition(int columnId, int positionIndex);
        Column GetColumnById(int id);
    }
}