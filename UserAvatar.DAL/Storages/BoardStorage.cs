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
        private readonly IColumnStorage _columnStorage;

        public BoardStorage(UserAvatarContext dbContext, IColumnStorage columnStorage)
        {
            _dbContext = dbContext;
            _columnStorage = columnStorage;
        }

        public async System.Threading.Tasks.Task CreateBoardAsync(Board board)
        {
            _dbContext.Boards.Add(board);

            await _dbContext.SaveChangesAsync();

            await AddAsMember(board.OwnerId, board.Id);

            await _dbContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoardsAsync(int userId)
        {
            return await _dbContext.Boards
                .Include(board => board.Members)
               .Where(board => board.Members.Any(member => member.UserId == userId))
               .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Board> GetBoardAsync(int userId, int boardId)
        {
            /*return await _dbContext.Boards
                .FirstOrDefaultAsync(board => board.Id == boardId && board.OwnerId == userId && board.isDeleted == false);*/
            
            return await _dbContext.Boards
                .Include(x=>x.Members)
                .FirstOrDefaultAsync(board => board.Id == boardId && board.Members
                    .Any(member=>member.Id==userId));
        }

        public async System.Threading.Tasks.Task UpdateAsync(int userId, Board board)
        {
            _dbContext.Entry(board).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId)
        {
            var board = _dbContext.Boards.First(board => board.OwnerId == userId && board.Id == boardId);

            board.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }

        public bool DoesUserHasBoard(int userId, string title)
        {
            return _dbContext.Users
                .Include(user => user.Boards)
                    .Any(user => user.Id == userId && user.Boards.
                        Any(board => board.Title == title));
        }

        public bool IsOwnerBoard(int userId, int boardId)
        {
            var count = _dbContext.Boards
                .Where(board => board.OwnerId == userId && board.Id == boardId)
                .Count();
               
            if (count == 0) return false;

            return true;
        }

        public bool IsUserBoard(int userId, int boardId)
        {
            var count = _dbContext.Boards
                .Include(board => board.Members)
                .Where(board => board.Id == boardId && board.Members
                .Any(member => member.Id == userId))
                .Count();

            if (count == 0) return false;

            return true;
        }

        public async System.Threading.Tasks.Task AddAsMember(int userId, int boardId)
        {
            if (IsMemberExist(userId, boardId)) throw new SystemException();

            var member = new Member()
            {
                UserId = userId,
                BoardId = boardId
            };

            _dbContext.Members.Add(member);

            await _dbContext.SaveChangesAsync();
        }

        public bool IsMemberExist(int userId, int boardId)
        {
            return _dbContext.Members.Any(member => member.UserId == userId && member.BoardId == boardId);
        }
    }
}