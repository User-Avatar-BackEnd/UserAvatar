using System;
using System.Linq;
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
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<Card> CreateAsync(Card card)
        {
            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();
            //*************************
            //Todo: FIX LINE 34. COLUMN C0.ISDELETED DOES NOT EXISTS;
            return await _dbContext.Cards
                .Include(x => x.Column)
                .Include(x => x.Responsible)
                .Include(x => x.Comments)
                .FirstAsync(x => x.Id == card.Id);
        }

        public async Task<int> GetCardsCountInColumnAsync(int columnId)
        {
            var column = await _dbContext.Columns
                .Include(x => x.Cards)
                .FirstOrDefaultAsync(x => x.Id == columnId &&!x.IsDeleted);
            //!x.IsDeleted???

            if (column == null) throw new Exception(); //column dosent exist

            return column.Cards.Where(x => !x.IsDeleted).Count();
        }

        public async Task<int> GetBoardIdAsync(int cardId)
        {
           var card = await _dbContext.Cards
                .Include(x => x.Column)
                .FirstOrDefaultAsync(x => x.Id == cardId);

            return card.Column.BoardId;
        }

        public async Task DeleteAsync(int cardId)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId);

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
    }
}
