﻿using System;
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
    public class BoardService : IBoardService
    {
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public BoardService(IBoardStorage boardStorage, IMapper mapper)
        {
            _boardStorage = boardStorage;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<BoardModel>>> GetAllBoardsAsync(int userId)
        {
            var boards = await _boardStorage.GetAllBoardsAsync(userId);

            var boardsModel = _mapper.Map<IEnumerable<Board>, IEnumerable<BoardModel>>(boards);

            return new Result<IEnumerable<BoardModel>>(boardsModel);
        }

        public async Task<int> CreateBoardAsync(int userId, string title)
        {
            var board = new Board()
            {
                Title = title,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            var boards = await GetAllBoardsAsync(userId);

            //todo:config
            if (boards.Value.Count() >= 10)
            {
                return ResultCode.MaxBoardCount;
            }

            await _boardStorage.CreateBoardAsync(board);
            return ResultCode.Success;
        }

        public async Task<Result<BoardModel>> GetBoardAsync(int userId, int boardId)
        {
            var board = await _boardStorage.GetBoardAsync(boardId);
            if (board == null)
            {
                return new Result<BoardModel>(ResultCode.NotFound);
            }

            var permission = await _boardStorage.IsUserBoardAsync(userId, boardId);
            if (!permission)
            {
                return new Result<BoardModel>(ResultCode.Forbidden);
            }

            return new Result<BoardModel>(_mapper.Map<Board, BoardModel>(board));
        }

        public async Task<int> RenameBoardAsync(int userId, int boardId, string title)
        {
            var board = await _boardStorage.GetBoardAsync(boardId);
            if (board == null)
            {
                return ResultCode.NotFound;
            }

            var permission = await _boardStorage.IsUserBoardAsync(userId, boardId);
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
            if (!await _boardStorage.IsOwnerBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }
            await _boardStorage.DeleteBoardAsync(userId, boardId);
            return ResultCode.Success;
        }
    }
}