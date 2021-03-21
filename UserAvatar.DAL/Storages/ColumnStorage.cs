using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.DAL.Storages
{
    public class ColumnStorage : IColumnStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        private static readonly SemaphoreSlim LockSlim = new SemaphoreSlim(1, 3);

        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public async Task Create(Column column)
        {
            await LockSlim.WaitAsync();
            try
            {
                var thisBoard = await _userAvatarContext.Boards.FindAsync(column.BoardId);
                if (thisBoard == null)
                    throw new Exception();

                var columnCount = _userAvatarContext.Columns
                    .Count(x => x.BoardId == column.BoardId);
                column.Board = thisBoard;
                column.Index = columnCount;

                await _userAvatarContext.Columns.AddAsync(column);
                await _userAvatarContext.SaveChangesAsync();
            }
            finally
            {
                LockSlim.Release();
            }
            
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

        public async Task ChangePosition(int columnId, int newIndex)
        {
            var thisColumn = GetColumnById(columnId);
            
            var columnList = _userAvatarContext.Columns
                .Where(x => x.BoardId == thisColumn.BoardId && x.Id != thisColumn.Id);

            
            var previousIndex = thisColumn.Index;
            thisColumn.Index = newIndex;
            
            if (!PositionAlgorithm(previousIndex, newIndex, columnList))
                throw new Exception();
                
            await _userAvatarContext.SaveChangesAsync();
        }
        private static bool PositionAlgorithm(int previousIndex, int newIndex, IQueryable<Column> columnList)
        {
            if(previousIndex - newIndex == 0)
                return true;
            if (newIndex < 0 || newIndex > columnList.Count())
                return false;
            
            foreach (var column in columnList)
                switch (previousIndex - newIndex) 
                { 
                    case < 0: 
                    { 
                        if (column.Index <= newIndex && column.Index >= previousIndex) 
                        {
                            column.Index--;
                        }
                        break; 
                    } 
                    case > 0: 
                    { 
                        if (column.Index <= previousIndex && column.Index >= newIndex)
                        {
                            column.Index++;
                        }
                        break; 
                    } 
                }
            return true;
        }

        public Column GetColumnById(int id)
        {
            return _userAvatarContext.Columns.Find(id);
        }
    }
}  