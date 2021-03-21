﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly ITaskStorage _taskStorage;

        private static readonly SemaphoreSlim LockSlim = new(1, 3);

        public ColumnStorage(UserAvatarContext userAvatarContext, ITaskStorage taskStorage)
        {
            _userAvatarContext = userAvatarContext;
            _taskStorage = taskStorage;
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
            //todo
            //collection.Select(c => {c.PropertyToSet = value; return c;}).ToList();
            //column.Select(c => {c.PropertyToSet = value; return c;}).ToList();
            _userAvatarContext.SaveChanges();
        }

        public async Task RecurrentlyDelete(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                column.isDeleted = true;
            }
            //todo: make recurrently 'isDeleted' tasks!
            await _userAvatarContext.SaveChangesAsync();
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