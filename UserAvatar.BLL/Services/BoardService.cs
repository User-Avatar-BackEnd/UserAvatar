using System;
using System.Collections.Generic;
using AutoMapper;
using UserAvatar.BLL.Models;
using UserAvatar.BLL.Services.Interfaces;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.BLL.Services
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

        public async System.Threading.Tasks.Task<IEnumerable<BoardModel>> GetAllBoardsAsync(int userId)
        {
            var boards = await _boardStorage.GetAllBoardsAsync(userId);

            return _mapper.Map<IEnumerable<Board>, IEnumerable<BoardModel>>(boards);
        }

        public async System.Threading.Tasks.Task<bool> CreateBoardAsync(int userId, string title)
        {
            var board = new Board()
            {
                Title = title,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            var successfullyCreated = await _boardStorage.CreateBoardAsync(userId, board);

            if (!successfullyCreated) throw new Exception();

            return true;
        }

        public async System.Threading.Tasks.Task<BoardModel> GetBoardAsync(int userId, int boardId)
        {
           var board = await _boardStorage.GetBoardAsync(userId, boardId);

            if (board == null) throw new Exception();

           return _mapper.Map<Board, BoardModel>(board);
        }

        public async System.Threading.Tasks.Task<bool> RenameBoardAsync(int userId, int boardId, string title)
        {
            var board = await _boardStorage.GetBoardAsync(userId, boardId);

            if (board == null) throw new Exception();

            board.Title = title;

           await _boardStorage.UpdateAsync(board);

            return true;
        }

        public async System.Threading.Tasks.Task<bool> DeleteBoardAsync(int userId, int boardId)
        {
            return await _boardStorage.DeleteBoardAsync(userId, boardId);
        }
    }
}