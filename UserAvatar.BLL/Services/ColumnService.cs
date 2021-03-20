using System;
using UserAvatar.BLL.Services.Interfaces;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.BLL.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnStorage _columnStorage;
        public ColumnService(IColumnStorage columnStorage)
        {
            _columnStorage = columnStorage;
        }

        public void Create(int boardId, string title)
        {
            var newColumn = new Column
            {
                Title = title,
                BoardId = boardId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            
            _columnStorage.Create(newColumn);
        }
        
        public void ChangePosition(int columnId, int positionIndex)
        {
            _columnStorage.ChangePosition(columnId,positionIndex);
        }

        public void Delete(int columnId)
        {
            _columnStorage.DeleteApparent(columnId);
        }

    }
}