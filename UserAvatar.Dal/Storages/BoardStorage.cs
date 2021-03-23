using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task CreateBoardAsync(Board board)
        {
            await _dbContext.Boards.AddAsync(board);

            await _dbContext.SaveChangesAsync();

            await AddAsMemberAsync(board.OwnerId, board.Id);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int userId)
        {
            return await _dbContext.Boards
                .Include(board => board.Members)
               .Where(board => board.Members.Any(member => member.UserId == userId))
               .ToListAsync();
        }

        public async Task<Board> GetBoardAsync(int userId, int boardId)
        {
            if (!await IsUserBoardAsync(userId, boardId)) return null;

            return await _dbContext.Boards
                .Include(x => x.User)
                .Include(x => x.Members)
                .Include(x => x.Columns)
                .ThenInclude(x=> x.Cards)
                .ThenInclude(x=> x.Comments)
                .FirstOrDefaultAsync(board => board.Id == boardId);
        }

        public async Task UpdateAsync(int userId, Board board)
        {
            _dbContext.Entry(board).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteBoardAsync(int userId, int boardId)
        {
            var board = await _dbContext.Boards
                .Include(x=>x.Columns)
                .FirstAsync(board => board.OwnerId == userId && board.Id == boardId);

            board.IsDeleted = true;

            await _columnStorage.RecurrentlyDeleteAsync(board.Columns);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DoesUserHasBoardAsync(int userId, string title)
        {
            return await _dbContext.Users
                .Include(user => user.Boards)
                    .AnyAsync(user => user.Id == userId && user.Boards.
                        Any(board => board.Title == title));
        }

        public async Task<bool> IsOwnerBoardAsync(int userId, int boardId)
        {
            var count = await _dbContext.Boards
                .Where(board => board.OwnerId == userId && board.Id == boardId)
                .CountAsync();
               
            if (count == 0) return false;

            return true;
        }

        public async Task<bool> IsUserBoardAsync(int userId, int boardId)
        {
            var count =  await _dbContext.Members.CountAsync(x => x.BoardId == boardId && x.UserId == userId);

            if (count == 0) return false;

            return true;
        }

        public async Task AddAsMemberAsync(int userId, int boardId)
        {
            if (await IsMemberExistAsync(userId, boardId)) throw new SystemException();

            var member = new Member()
            {
                UserId = userId,
                BoardId = boardId
            };

            await _dbContext.Members.AddAsync(member);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsMemberExistAsync(int userId, int boardId)
        {
            return await _dbContext.Members.AnyAsync(member => member.UserId == userId && member.BoardId == boardId);
        }
    }
}