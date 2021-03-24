using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
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
       
        public ColumnService(
            IColumnStorage columnStorage,
            IMapper mapper,
            IBoardStorage boardStorage,
            IOptions<LimitationOptions> limitations)
        {
            _columnStorage = columnStorage;
            _mapper = mapper;
            _boardStorage = boardStorage;
            _limitations = limitations.Value;
        }

        //todo: add userId

        public async Task<Result<ColumnModel>> CreateAsync(
            int userId, int boardId, string title)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<ColumnModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<ColumnModel>(ResultCode.Forbidden);
            }

            var newColumn = new Column
            {
                Title = title,
                BoardId = boardId,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow
            };

            var column = await _columnStorage.CreateAsync(newColumn);

            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(column));
        }

        public async Task<int> ChangePositionAsync(
            int userId, int boardId, int columnId, int positionIndex)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            /*if (!_columnStorage.IsUserInBoardByColumnId(userId, columnId))
            {
                return ResultCode.Forbidden;
            }*/
            
            if (await _columnStorage.GetColumnsCountInBoardAsync(columnId) > _limitations.MaxColumnCount)
            {
                return ResultCode.MaxColumnCount;
            }

            await _columnStorage.ChangePositionAsync(columnId, positionIndex);

            return ResultCode.Success;
        }

        public async Task<int> DeleteAsync(
            int userId, int boardId, int columnId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            if (!await _boardStorage.IsBoardColumn(boardId, columnId))
            {
                return ResultCode.NotFound;
            }

            // IsUserInBoardByColumnId maybe may be async

            if (!_columnStorage.IsUserInBoardByColumnId(userId, columnId))
            {
                return ResultCode.Forbidden;
            }
            
            //todo: implement in storage recursive deletion

            await _columnStorage.DeleteApparentAsync(columnId);

            return ResultCode.Success;
        }

        public async Task<int> UpdateAsync(
            int userId, int boardId, int columnId, string title)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            if (!await _boardStorage.IsBoardColumn(boardId, columnId))
            {
                return ResultCode.NotFound;
            }

            var thisColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            if (thisColumn is null)
            {
                return ResultCode.NotFound;
            }

            thisColumn.Title = title;
            await _columnStorage.UpdateAsync(thisColumn);

            return ResultCode.Success;
        }

        public async Task<Result<ColumnModel>> GetColumnByIdAsync(
            int userId, int boardId, int columnId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<ColumnModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<ColumnModel>(ResultCode.Forbidden);
            }

            if (!await _boardStorage.IsBoardColumn(boardId, columnId))
            {
                return new Result<ColumnModel>(ResultCode.NotFound);
            }

            var foundColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            return foundColumn is null 
                ? new Result<ColumnModel>(ResultCode.NotFound) 
                : new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(foundColumn));
        }
    }
}