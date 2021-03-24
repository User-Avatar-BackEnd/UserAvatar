using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class CardStorage : ICardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public CardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Card> GetByIdAsync(int id)
        {
            return await _dbContext.Cards
                .Include(x=>x.Column)
                .Include(x=>x.Responsible)
                .Include(x=>x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id );
            
            // Removed && !x.IsDeleted. I think this does not have meaning
        }
        public async Task<Card> CreateAsync(Card card)
        {
            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();
            
            return await _dbContext.Cards
                .Include(x => x.Column)
                .Include(x => x.Responsible)
                .Include(x => x.Comments)
                .FirstAsync(x => x.Id == card.Id);
        }

        public async Task<int> GetCardsCountInColumnAsync(int columnId)
        {
            // Here was refactored to make it async
            var column = await _dbContext.Columns
                .Include(x => x.Cards)
                .CountAsync(x => x.Id == columnId);

            // Removed && !x.IsDeleted. I think this does not have meaning
            if (column == 0) throw new Exception(); //column doesn't exist

            return column;
        }

        public async Task<int> GetBoardIdAsync(int cardId)
        {
            var card = await _dbContext.Cards
                .Include(x => x.Column)
                .FirstOrDefaultAsync(x => x.Id == cardId);

            return card?.Column.BoardId ?? 0;
        }

        public async Task DeleteAsync(int cardId)
        {
            //Todo: Add recursive deletion
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(x => x.Id == cardId);

            if (card == null) throw new Exception();

            card.IsDeleted = true;
            //todo: comments soft delete
           await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Card card)
        {
            _dbContext.Entry(card).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> GetCardIdByColumnId(int columnId)
        {
            return await Task.FromResult(_dbContext.Cards.FirstOrDefaultAsync(x => x.ColumnId == columnId).Id);
        }

        public async Task<bool> IsCardComment(int cardId, int commentId)
        {
            return await _dbContext.Comments
                .AnyAsync(x => x.Id == commentId && x.CardId == cardId);
        }
    }
}
