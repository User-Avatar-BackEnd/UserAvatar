using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace UserAvatar.Dal.Storages
{
    public class ColumnStorage : IColumnStorage
    {
        private readonly UserAvatarContext _userAvatarContext;
        //private readonly ICardStorage _taskStorage;

        private static readonly SemaphoreSlim LockSlim = new(1, 1);
        private static readonly SemaphoreSlim LockSlimForRecheck = new(1, 1);

        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
           _userAvatarContext = userAvatarContext;
        }

        public async Task<Column> CreateAsync(Column column)
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

                // Please do not change or userAvatarContext would be disposed after first method call
                await _userAvatarContext.Columns.AddAsync(column);
                await  _userAvatarContext.SaveChangesAsync();
                return column;  
            }
            finally
            {
                LockSlim.Release();
            }
        }

        // there can be change on async as well
        public bool IsUserInBoardByColumnId(int userId,int columnId)
        {
            /*var zzz = _userAvatarContext.Columns.Where(x => x.Id == columnId)
                .Include(x=> x.Board)
                .ThenInclude(x=> x.Members.Count(x => x.UserId == userId));*/

            // why don't you want to return in at once instead creating a variable and returning this variable?


            // Дима сказал: там инклуд не нужен вроде, если вы не вытягиваете данные наружу, а я не уверенна ибо не делала это :D
            // Поэтому осталю пока это тут)


            var zzz = _userAvatarContext.Boards
                .Include(x => x.Columns)
                .Include(x => x.Members)
                .Any(x=> x.Columns.Any(x=> x.Id == columnId) && x.Members.Any(x=> x.UserId == userId));

            return zzz;
        }
        
        public async Task DeleteApparentAsync(int columnId)
        {
            var column = await GetColumnByIdAsync(columnId);
            if (column.IsDeleted)
                throw new Exception($"Already Deleted!{columnId}");
            
            column.IsDeleted = true;
            var columnList = InternalGetAllColumns(column);
            await RecheckPositionAsync(columnList.ToList(),column.Index);

            //column.Index = -1;
            //_userAvatarContext.Update(column);
            // recurrently delete all tasks
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task RecurrentlyDeleteAsync(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                column.IsDeleted = true;
            }
            //todo: make recurrently 'isDeleted' tasks!
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<List<Column>> GetAllColumnsAsync(int boardId)
        {
            //todo: maybe change
            // I don't know if i can change there?
            var card = Task.Factory.StartNew(() =>
                _userAvatarContext.Columns
                    .Include(x => x.Cards).Where(x => x.Board.Id == boardId)
                    .OrderBy(x => x.Index)
                    .ToList());

            return await card;
        }

        public async Task UpdateAsync(Column column)
        {
            _userAvatarContext.Entry(column).State = EntityState.Modified;
            await _userAvatarContext.SaveChangesAsync();
        }

        private IEnumerable<Column> InternalGetAllColumns(Column column)
        {
            return _userAvatarContext.Columns
                .Where(x => x.BoardId == column.BoardId);
        }

        public async Task ChangePositionAsync(int columnId, int newIndex)
        {
            var thisColumn = await GetColumnByIdAsync(columnId);
            
            var columnList = _userAvatarContext.Columns
                .Where(x => x.BoardId == thisColumn.BoardId && x.Id != thisColumn.Id && !x.IsDeleted);

            
            var previousIndex = thisColumn.Index;
            thisColumn.Index = newIndex;
            
            if (!PositionAlgorithm(previousIndex, newIndex, columnList))
                throw new Exception();
            
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<Column> GetColumnByIdAsync(int id)
        {
            return await _userAvatarContext.Columns.FindAsync(id);
        }

        private static async Task RecheckPositionAsync(List<Column> columnList, int deletedPosition)
        {
            await LockSlimForRecheck.WaitAsync();
            try
            {
                if (deletedPosition == columnList.Count)
                    return;
                foreach (var column in columnList.Where(column => column.Index > deletedPosition))
                {
                    column.Index--;
                }
            }
            finally
            {
                LockSlimForRecheck.Release();
            }
        }

        private static bool PositionAlgorithm(int previousIndex, int newIndex, IQueryable<Column> columnList)
        {
            if(previousIndex - newIndex == 0)
                return true;
            if (newIndex < 0 || newIndex > columnList.Count() + 1)
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
    }
}  