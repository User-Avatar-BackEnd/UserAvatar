using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        //private readonly ICardStorage _taskStorage;

        private static readonly SemaphoreSlim LockSlim = new(1, 1);

        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
           _userAvatarContext = userAvatarContext;
        }

        public async Task<Column> Create(Column column)
        {
            await LockSlim.WaitAsync();
            try
            {
                //var thisBoard = _userAvatarContext.Boards.FindAsync(column.BoardId).Result;
                var thisBoard = _userAvatarContext.Boards.FirstOrDefault(x => x.Id == column.BoardId);
                if (thisBoard == null)
                    throw new Exception();

                var columnCount = _userAvatarContext.Columns
                    .Count(x => x.BoardId == column.BoardId);
                column.Board = thisBoard;
                column.Index = columnCount;

                // Please do not change or userAvatarContext would be disposed after first method call
                _userAvatarContext.Columns.Add(column);
                _userAvatarContext.SaveChanges();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                //todo: delete
            }
            finally
            {
                LockSlim.Release();
            }
            return column;

        }

        public bool IsUserInBoardByColumnId(int userId,int columnId)
        {
            /*var zzz = _userAvatarContext.Columns.Where(x => x.Id == columnId)
                .Include(x=> x.Board)
                .ThenInclude(x=> x.Members.Count(x => x.UserId == userId));*/

            var zzz = _userAvatarContext.Boards
                .Include(x => x.Columns)
                .Include(x => x.Members)
                .Any(x=> x.Columns.Any(x=> x.Id == columnId) && x.Members.Any(x=> x.UserId == userId));

            return zzz;
        }
        
        public async Task DeleteApparent(int columnId)
        {
            var column = await GetColumnById(columnId);
            if (column.IsDeleted)
                throw new Exception($"Already Deleted!{columnId}");
            column.IsDeleted = true;
            _userAvatarContext.Update(column);
            //collection.Select(c => {c.PropertyToSet = value; return c;}).ToList();
            //column.Select(c => {c.PropertyToSet = value; return c;}).ToList();
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task RecurrentlyDelete(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                column.IsDeleted = true;
            }
            //todo: make recurrently 'isDeleted' tasks!
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<IQueryable<Column>> GetAllColumns(int boardId)
        {
            //todo: maybe change
            var task = Task.Factory.StartNew(() =>
                _userAvatarContext.Columns.Where(x => x.Board.Id == boardId).OrderBy(x => x.Index));
            return await task;
            
        }

        public async Task Update(Column column)
        {
            _userAvatarContext.Entry(column).State = EntityState.Modified;
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task ChangePosition(int columnId, int newIndex)
        {
            var thisColumn = await GetColumnById(columnId);
            
            var columnList = _userAvatarContext.Columns
                .Where(x => x.BoardId == thisColumn.BoardId && x.Id != thisColumn.Id);

            
            var previousIndex = thisColumn.Index;
            thisColumn.Index = newIndex;
            
            if (!PositionAlgorithm(previousIndex, newIndex, columnList))
                throw new Exception();
            
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<Column> GetColumnById(int id)
        {
            return await _userAvatarContext.Columns.FindAsync(id);
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
    }
}  