using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class BoardStorage : IBoardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public BoardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async System.Threading.Tasks.Task<bool> CreateBoardAsync(int userId, Board board)
        {
            var boards = await GetAllBoardsAsync(userId);

            if (boards.Count() > 10) throw new Exception();

            if (DoesUserHasBoard(userId, board.Title)) throw new Exception();

            _dbContext.Boards.Add(board);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoardsAsync(int userId)
        {
             return await _dbContext.Boards
                .Include(x => x.Members)
                .Where(board => board.Members.Any(member => member.UserId == userId && member.isDeleted==false) && board.isDeleted == false)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Board> GetBoardAsync(int userId, int boardId)
        {
            return await _dbContext.Boards
                .FirstOrDefaultAsync(board => board.Id == boardId && board.OwnerId==userId && board.isDeleted == false);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Board board)
        {
            _dbContext.Entry(board).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<bool> DeleteBoardAsync(int userId, int boardId)
        {
            //if (!IsUsersBoard(userId, boardId)) throw new Exception();

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
        // todo: fix linq
        public bool IsUsersBoard(int userId, int boardId)
        {
            var count = _dbContext.Members
                .Include(x => x.Board)
                .Count(member => member.Id == userId && member.BoardId == boardId && member.Board.isDeleted == false && member.isDeleted ==false);

            if (count == 0) return false;

            return true;
        }
    }
}