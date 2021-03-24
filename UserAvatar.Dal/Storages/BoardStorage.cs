using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Z.EntityFramework.Plus;

namespace UserAvatar.Dal.Storages
{
    public class BoardStorage : IBoardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public BoardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateBoardAsync(Board board)
        {
            await _dbContext.Boards.AddAsync(board);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync(int userId)
        {
            return await _dbContext.Boards
                .Include(board => board.Members)
                .Where(board => board.Members.Any(member => member.UserId == userId))
                .ToListAsync();
        }

        public async Task<Board> GetBoardAsync(int boardId)
        {
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
        public async Task DeleteRecurrentlyBoardAsync(int boardId)
        {
            var thisBoard = await _dbContext.Boards.FindAsync(boardId);

            await _dbContext.Columns.Where(x => x.BoardId == boardId)
                .Include(x => x.Cards)
                .ThenInclude(x => x.Comments).SelectMany(x => x.Cards.SelectMany(card => card.Comments))
                .UpdateAsync(x => new Comment {IsDeleted = true});
            
            await _dbContext.Columns.Where(x=> x.BoardId == boardId)
                .Include(x=> x.Cards)
                .SelectMany(x=> x.Cards)
                .UpdateAsync(x => new Card {IsDeleted = true});
            
            await _dbContext.Columns.Where(x => x.BoardId == boardId)
                .UpdateAsync(x => new Column {IsDeleted = true});
            
            await _dbContext.Members.Where(x=> x.BoardId == boardId)
                .UpdateAsync(x => new Member {IsDeleted = true});
            
            thisBoard.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Member> GetMemberByIdAsync(int userId, int boardId)
        {
            return await _dbContext.Members
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BoardId == boardId);
        }

        /// <inheritdoc />
        public async Task<bool> IsOwnerBoardAsync(int userId, int boardId)
        {
            return await _dbContext.Boards
                .AnyAsync(board => board.OwnerId == userId && board.Id == boardId);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserBoardAsync(int userId, int boardId)
        {
            return await _dbContext.Members
                .AnyAsync(x => x.BoardId == boardId && x.UserId == userId);
            
        }

        /*public async Task AddAsMemberAsync(int userId, int boardId)
        {
            if (await IsMemberExistAsync(userId, boardId)) throw new SystemException();

            var member = new Member
            {
                UserId = userId,
                BoardId = boardId
            };

            await _dbContext.Members.AddAsync(member);

            await _dbContext.SaveChangesAsync();
        }*/
        
        public async Task AddAsMemberAsync(Member member)
        {
            await _dbContext.Members.AddAsync(member);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAllBoardsAsync(int userId)
        {
            return await _dbContext.Boards.CountAsync(x => x.OwnerId == userId);
        }

        public async Task UpdateMemberAsync(Member member)
        {
            _dbContext.Entry(member).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task<bool> IsBoardExistAsync(int boardId)
        {
            return await _dbContext.Boards
                .AnyAsync(board => board.Id == boardId);
        }

        public async Task<bool> IsBoardColumn(int boardId, int columnId)
        {
            return await _dbContext.Columns
                .AnyAsync(x => x.BoardId == boardId && x.Id == columnId);
        }

        //todo: think about this
        public async Task<bool> IsBoardCard(int boardId, int cardId)
        {
            return await _dbContext.Boards
                .Where(x=>x.Id == boardId)
                .Include(x=>x.Columns)
                .ThenInclude(x=> x.Cards)
                .AnyAsync(x => x.Columns
                    .Any(x=> x.Cards.
                        Any(x=>x.Id == cardId)));
        }
    }
}