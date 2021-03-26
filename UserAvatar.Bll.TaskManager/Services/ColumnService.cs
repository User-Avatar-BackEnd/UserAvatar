using System;
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

        //todo: add userId

        public async Task<Result<ColumnModel>> CreateAsync(
            int userId, int boardId, string title)
        {
            var validation = await ValidateUserColumnAsync(userId, boardId);
            if (validation != ResultCode.Success)
            {
                return new Result<ColumnModel>(validation);
            }

            if (await _columnStorage.GetColumnsCountInBoardAsync(boardId) > _limitations.MaxColumnCount)
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

            var column = await _columnStorage.CreateAsync(newColumn);

            _boardChangesService.DoChange(boardId, userId);

            return new Result<ColumnModel>(_mapper.Map<Column, ColumnModel>(column));
        }

        public async Task<int> ChangePositionAsync(
            int userId, int boardId, int columnId, int positionIndex)
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
            
            await _columnStorage.ChangePositionAsync(columnId, positionIndex);

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
            
            await _columnStorage.DeleteApparentAsync(columnId);

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
    }
}