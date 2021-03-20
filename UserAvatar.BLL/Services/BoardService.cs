using System;
using System.Collections.Generic;
using AutoMapper;
using UserAvatar.BLL.Models;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages;


namespace UserAvatar.BLL.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public BoardService(IBoardStorage boardStorage)
        {
            _boardStorage = boardStorage;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Board, BoardModel>()).CreateMapper();
        }

        public async System.Threading.Tasks.Task<IEnumerable<BoardModel>> GetAllBoards(int userId)
        {
            var boards = await _boardStorage.GetAllBoards(userId);

            return _mapper.Map<IEnumerable<Board>, IEnumerable<BoardModel>>(boards);
        }

        public async System.Threading.Tasks.Task<bool> CreateBoard(int userId, string title)
        {
            var board = new Board()
            {
                Title = title,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            var successfullyCreated = await _boardStorage.CreateBoard(userId, board);

            if (!successfullyCreated) throw new Exception();

            return true;
        }

        public async System.Threading.Tasks.Task<BoardModel> GetBoard(int userId, int boardId)
        {
           var board = await _boardStorage.GetBoard(userId, boardId);

            if (board == null) throw new Exception();

           return _mapper.Map<Board, BoardModel>(board);
        }

        public async System.Threading.Tasks.Task<bool> RenameBoard(int userId, int boardId, string title)
        {
            var board = await _boardStorage.GetBoard(userId, boardId);

            if (board == null) throw new Exception();

            board.Title = title;

           await _boardStorage.Update(board);

            return true;
        }

        public async System.Threading.Tasks.Task<bool> DeleteBoard(int userId, int boardId)
        {
            return await _boardStorage.DeleteBoard(userId, boardId);
        }
    }
}