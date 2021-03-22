using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using UserAvatar.Infrastructure.Exceptions;

namespace UserAvatar.Bll.Services
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

        public async System.Threading.Tasks.Task CreateBoardAsync(int userId, string title)
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
            if (boards.Count() >= 10)
            {
                throw new InformException("You already create maximum 10 boards");
            }

            if (_boardStorage.DoesUserHasBoard(userId, board.Title))
            {
                throw new InformException("You already create maximum 10 boards");
            }

            await _boardStorage.CreateBoardAsync(board);
        }

        public async System.Threading.Tasks.Task<BoardModel> GetBoardAsync(int userId, int boardId)
        {
            var board = await _boardStorage.GetBoardAsync(userId, boardId);

            if (board == null) throw new Exception();

            return _mapper.Map<Board, BoardModel>(board);
        }

        public async System.Threading.Tasks.Task RenameBoardAsync(int userId, int boardId, string title)
        {
            var board = await _boardStorage.GetBoardAsync(userId, boardId);

            if (board == null) throw new Exception("This board doesn't exist");

            var isSameBoardExist = _boardStorage.DoesUserHasBoard(userId, title);

            if (isSameBoardExist) throw new SystemException();

            board.Title = title;

           await _boardStorage.UpdateAsync(userId, board);
        }

        public async System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId)
        {
            if (!_boardStorage.IsOwnerBoard(userId, boardId)) throw new Exception();

            await _boardStorage.DeleteBoardAsync(userId, boardId);
        }
    }
}
