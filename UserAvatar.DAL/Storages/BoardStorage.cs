using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public class BoardStorage : IBoardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public BoardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async System.Threading.Tasks.Task<bool> CreateBoard(int userId, Board board)
        {
            var boards = await GetAllBoards(userId);

            if (boards.Count() > 10) throw new Exception();

            if (DoesUserHasBoard(userId, board.Title)) throw new Exception();

            _dbContext.Boards.Add(board);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoards(int userId)
        {
             return await _dbContext.Boards
                .Include(x => x.Members)
                .Where(board => board.Members.Any(member => member.UserId == userId && member.isDeleted==false) && board.isDeleted == false)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Board> GetBoard(int userId, int boardId)
        {
            return await _dbContext.Boards
                .FirstOrDefaultAsync(board => board.Id == boardId && board.OwnerId==userId && board.isDeleted == false);
        }

        public async System.Threading.Tasks.Task Update(Board board)
        {
            _dbContext.Entry(board).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<bool> DeleteBoard(int userId, int boardId)
        {
            if (!IsUsersBoard(userId, boardId)) throw new Exception();

            var board = _dbContext.Boards.First(board => board.Id == userId && board.Id==boardId);

            board.isDeleted = true;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        // does user has board with same title
        public bool DoesUserHasBoard(int userId,string title)
        {
            return _dbContext.Users
                .Include(user => user.Boards)
                    .Any(user =>user.Id==userId && user.Boards.
                        Any(board=> board.Title==title));
        }

        // does user has this board
        public bool IsUsersBoard(int userId, int boardId)
        {
            var count = _dbContext.Members.Include(x=> x.Board)
                .Where(member => member.Id == userId && member.BoardId == boardId && member.Board.isDeleted == false && member.isDeleted ==false)
                .Count();

            if (count == 0) return false;

            return true;
        }
    }
}