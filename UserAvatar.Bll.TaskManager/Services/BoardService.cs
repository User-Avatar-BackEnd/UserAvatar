using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BoardService : IBoardService
    {
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;
        private readonly LimitationOptions _limitations;

        public BoardService(IBoardStorage boardStorage, 
            IMapper mapper, 
            IOptions<LimitationOptions> limitations)
        {
            _boardStorage = boardStorage;
            _mapper = mapper;
            _limitations = limitations.Value;
        }

        public async Task<Result<IEnumerable<BoardModel>>> GetAllBoardsAsync(int userId)
        {
            var boards = await _boardStorage
                .GetAllBoardsAsync(userId);

            var boardsModel = _mapper
                .Map<IEnumerable<Board>, IEnumerable<BoardModel>>(boards);

            return new Result<IEnumerable<BoardModel>>(boardsModel);
        }

        public async Task<Result<BoardModel>> CreateBoardAsync(int userId, string title)
        {
            var board = new Board
            {
                Title = title,
                OwnerId = userId,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow,
                ModifiedBy = 0
            };
            var boards = await _boardStorage.CountAllBoardsAsync(userId);

            if (boards >= _limitations.MaxBoardCount)
            {
                return new Result<BoardModel>(ResultCode.MaxBoardCount);
            }
            
            await _boardStorage.CreateBoardAsync(board);
            await _boardStorage.AddAsMemberAsync(new Member
            {
                UserId = userId,
                BoardId = board.Id,
            });
            return new Result<BoardModel>(_mapper.Map<Board,BoardModel>(board), EventType.CreateBoard);
        }

        public async Task<Result<BoardModel>> GetBoardAsync(int userId, int boardId)
        {
            var board = await _boardStorage.GetBoardAsync(boardId);
            
            if (board == null)
            {
                return new Result<BoardModel>(ResultCode.NotFound);
            }

            var permission = await _boardStorage.IsUserBoardAsync(userId, boardId);
            return !permission 
                ? new Result<BoardModel>(ResultCode.Forbidden) 
                : new Result<BoardModel>(_mapper.Map<Board, BoardModel>(board));
        }

        public async Task<int> RenameBoardAsync(int userId, int boardId, string title)
        {
            var board = await _boardStorage.GetBoardAsync(boardId);
            
            if (board == null)
            {
                return ResultCode.NotFound;
            }

            var permission = await _boardStorage.IsOwnerBoardAsync(userId, boardId);
            if (!permission)
            {
                return ResultCode.Forbidden;
            }

            board.Title = title;

            await _boardStorage.UpdateAsync(userId, board);
            return ResultCode.Success;
        }

        public async Task<int> DeleteBoardAsync(int userId, int boardId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
                return ResultCode.NotFound;

            if (await _boardStorage.IsOwnerBoardAsync(userId,boardId))
            {
                await _boardStorage.DeleteRecurrentlyBoardAsync(boardId);
            } 
            else if (await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                var thisMember = await _boardStorage.GetMemberByIdAsync(userId, boardId);
                thisMember.IsDeleted = true;
                await _boardStorage.UpdateMemberAsync(thisMember);
            }
            else
            {
                return ResultCode.Forbidden;
            }
            
            return ResultCode.Success;
        }

        public async Task<int> DeleteMemberFromBoardAsync(int userId, int toDeleteUserId, int boardId)
        {
            var currentBoard = await _boardStorage.GetBoardAsync(boardId);

            if (currentBoard == null
                || currentBoard.OwnerId != userId
                || currentBoard.Members.All(x => x.UserId != toDeleteUserId))
                return ResultCode.BadRequest;

            var thisMember = await _boardStorage.GetMemberByIdAsync(toDeleteUserId, boardId);
            thisMember.IsDeleted = true;
            
            await _boardStorage.UpdateMemberAsync(thisMember);
            return ResultCode.Success;
        }
    }
}
