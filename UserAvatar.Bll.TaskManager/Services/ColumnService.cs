using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        public ColumnService(IColumnStorage columnStorage, IMapper mapper, IBoardStorage boardStorage)
        {
            _columnStorage = columnStorage;
            _mapper = mapper;
            _boardStorage = boardStorage;
        }

        //todo: add userId

        public async Task<Result<ColumnModel>> CreateAsync(int userId, int boardId, string title)
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
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };

            var column = await _columnStorage.CreateAsync(newColumn);

            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(column));
        }

        public async Task<int> ChangePositionAsync(int userId, int boardId, int columnId, int positionIndex)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            // что у этой борды есть такая колонка

            // todo: new method !
            // NotFound

            // IsUserInBoardByColumnId maybe may be async

            if (!_columnStorage.IsUserInBoardByColumnId(userId, columnId))
            {
                return ResultCode.Forbidden;
            }

            await _columnStorage.ChangePositionAsync(columnId, positionIndex);

            return ResultCode.Success;
        }

        public async Task<int> DeleteAsync(int userId, int boardId, int columnId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            // check: что у этой борды есть такая колонка
            // todo: new method !
            // NotFound

            // IsUserInBoardByColumnId maybe may be async

            if (!_columnStorage.IsUserInBoardByColumnId(userId, columnId))
            {
                return ResultCode.Forbidden;
            }

            await _columnStorage.DeleteApparentAsync(columnId);

            return ResultCode.Success;
        }

        public async Task<int> UpdateAsync(int userId, int boardId, int columnId, string title)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            // что у этой борды есть такая колонка
            // todo: new method !
            // NotFound

            var thisColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            if (thisColumn is null)
            {
                return ResultCode.NotFound;
            }

            thisColumn.Title = title;
            await _columnStorage.UpdateAsync(thisColumn);

            return ResultCode.Success;
        }

        public async Task<Result<ColumnModel>> GetColumnByIdAsync(int userId, int boardId, int columnId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<ColumnModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<ColumnModel>(ResultCode.Forbidden);
            }

            // что у этой борды есть такая колонка
            // что у этой борды есть такая колонка
            // todo: new method !
            // NotFound


            var foundColumn = await _columnStorage.GetColumnByIdAsync(columnId);

            if (foundColumn is null)
            {
                return new Result<ColumnModel>(ResultCode.NotFound);
            }

            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(foundColumn));
        }

        public async Task<Result<List<ColumnModel>>> GetAllColumnsAsync(int userId, int boardId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<List<ColumnModel>>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<List<ColumnModel>>(ResultCode.Forbidden);
            }

            var allColumns = await _columnStorage.GetAllColumnsAsync(boardId);

            if (allColumns.Count() < 0)
            {
                return new Result<List<ColumnModel>>(ResultCode.NotFound);
            }

            return new Result<List<ColumnModel>>(_mapper.Map<List<Column>, List<ColumnModel>>(allColumns));
        }
    }
}