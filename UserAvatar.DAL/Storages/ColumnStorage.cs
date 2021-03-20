using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.DAL.Storages
{
    public class ColumnStorage : IColumnStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public void Create(Column column)
        {
            var thisBoard = _userAvatarContext.Boards.Find(column.BoardId);
            if (thisBoard == null)
                throw new Exception();
            
            var columnCount = _userAvatarContext.Columns
                .Count(x => x.BoardId == column.BoardId);
            column.Board = thisBoard;
            column.Index = columnCount;
            
            _userAvatarContext.Columns.Add(column);
            _userAvatarContext.SaveChanges();
        }

        public void DeleteApparent(int columnId)
        {
            var column = GetColumnById(columnId);
            column.isDeleted = true;
            _userAvatarContext.SaveChanges();
        }

        public void Update(Column column)
        {
            _userAvatarContext.Entry(column).State = EntityState.Modified;
            _userAvatarContext.SaveChanges();
        }

        public void ChangePosition(int columnId, int positionIndex)
        {
            var thisColumn = GetColumnById(columnId);
            if (thisColumn == null)
                throw new Exception();
            var columnList = _userAvatarContext.Columns
                .Include(x => x.Board).Where(x => x.BoardId == thisColumn.BoardId);
            thisColumn.Index = positionIndex;
            foreach (var column in columnList)
            {
                if (column.Index >= thisColumn.Index)
                {
                    column.Index++;
                }
            }

            _userAvatarContext.SaveChanges();
        }

        public Column GetColumnById(int id)
        {
            return _userAvatarContext.Columns.Find(id);
        }
    }
}