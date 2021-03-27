using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnStorage _columnStorage;
        private readonly IMapper _mapper;
        private readonly IBoardStorage _boardStorage;
        private readonly LimitationOptions _limitations;
        private readonly IBoardChangesService _boardChangesService;
        
        private static readonly SemaphoreSlim LockSlim = new(1, 1);
        private static readonly SemaphoreSlim LockSlimForRecheck = new(1, 1);

        public ColumnService(
            IColumnStorage columnStorage,
            IMapper mapper,
            IBoardStorage boardStorage,
            IOptions<LimitationOptions> limitations,
            IBoardChangesService boardChangesService)
        {
            _columnStorage = columnStorage;
            _mapper = mapper;
            _boardStorage = boardStorage;
            _limitations = limitations.Value;
            _boardChangesService = boardChangesService;
        }
        public async Task<Result<ColumnModel>> CreateAsync(int userId, int boardId, string title)
        {
            var validation = await ValidateUserColumnAsync(userId, boardId);
            if (validation != ResultCode.Success)
            {
                return new Result<ColumnModel>(validation);
            }

            if (await _columnStorage.GetColumnsCountInBoardAsync(boardId) >= _limitations.MaxColumnCount)
            {
                return new Result<ColumnModel>(ResultCode.MaxColumnCount);
            }

            var newColumn = new Column
            {
                Title = title,
                BoardId = boardId,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow
            };
            
            await LockSlim.WaitAsync();
            try
            {
                var thisBoard = await _boardStorage.GetBoardAsync(boardId);
                var columnCount = await _columnStorage.CountColumnsInBoardAsync(boardId);
                
                newColumn.Board = thisBoard;
                newColumn.Index = columnCount;
                
                await _columnStorage.AddColumnAsync(newColumn);
            }
            finally
            {
                LockSlim.Release();
            }
            
            _boardChangesService.DoChange(boardId, userId);
            
            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(newColumn));
        }
        public async Task<int> ChangePositionAsync(int userId, int boardId, int columnId, int positionIndex)
        {
            var validation = await ValidateUserColumnAsync(userId, boardId, columnId);
            
            if (validation != ResultCode.Success)
            {
                return validation;
            }

            if (await _columnStorage.GetColumnsCountInBoardAsync(boardId) - 1 < positionIndex)
            {
                return ResultCode.BadRequest;
            }
            
            var thisColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            var columnList = await _columnStorage.GetAllColumnsExceptThis(thisColumn);
            
            var previousIndex = thisColumn.Index;
            thisColumn.Index = positionIndex;

            if (!PositionAlgorithm(previousIndex, positionIndex, columnList))
                return ResultCode.BadRequest;
            //throw new Exception(); I will change into something else

            await _columnStorage.Update();

            _boardChangesService.DoChange(boardId, userId);

            return ResultCode.Success;
        }
        public async Task<int> DeleteAsync(int userId, int boardId, int columnId)
        {
            var validation = await ValidateUserColumnAsync(userId, boardId, columnId);
            if (validation != ResultCode.Success)
            {
                return validation;
            }
            
            var column = await _columnStorage.GetColumnByIdAsync(columnId);
            if (column.IsDeleted)
                return ResultCode.BadRequest;
            
            var columnList = await _columnStorage.InternalGetAllColumns(column);
            await RecheckPositionAsync(columnList,column.Index);

            await _columnStorage.DeleteApparentAsync(column);
            
            _boardChangesService.DoChange(boardId, userId);

            return ResultCode.Success;
        }
        public async Task<int> UpdateAsync(int userId, int boardId, int columnId, string title)
        {   
            var validation = await ValidateUserColumnAsync(userId, boardId, columnId);
            if (validation != ResultCode.Success)
            {
                return validation;
            }

            var thisColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            thisColumn.Title = title;

            await _columnStorage.UpdateAsync(thisColumn);

            _boardChangesService.DoChange(boardId, userId);

            return ResultCode.Success;
        }
        public async Task<Result<ColumnModel>> GetColumnByIdAsync(int userId, int boardId, int columnId)
        {
            int validation = await ValidateUserColumnAsync(userId, boardId, columnId);
            if (validation != ResultCode.Success)
            {
                return new Result<ColumnModel>(validation);
            }

            var foundColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(foundColumn));
        }
        private async Task<int> ValidateUserColumnAsync(int userId, int boardId, int? columnId=null)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }
            
            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            if (columnId != null)
            {
                if (!await _boardStorage.IsBoardColumnAsync(boardId, (int)columnId))
                {
                    return ResultCode.NotFound;
                }
            }

            return ResultCode.Success;
        }
        private static bool PositionAlgorithm(int previousIndex, int newIndex, List<Column> columnList)
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
    }
}