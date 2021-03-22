using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.DAL.Storages
{
    public class CardStorage : ICardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public CardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Card GetById(int id)
        {
            return _dbContext.Cards
                .Include(x=>x.Column)
                .Include(x=>x.Responsible)
                .Include(x=>x.Comments)
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public Card Create(Card card)
        {
            _dbContext.Cards.Add(card);
            _dbContext.SaveChanges();
            //*************************
            //Todo: FIX LINE 34. COLUMN C0.ISDELETED DOES NOT EXISTS;
            return _dbContext.Cards
                .Include(x => x.Column)
                .Include(x => x.Responsible)
                .Include(x => x.Comments)
                .First(x => x.Id == card.Id);
        }

        public int GetCardsCountInColumn(int columnId)
        {
            var column = _dbContext.Columns
                .Include(x => x.Cards)
                .Where(x => x.Id == columnId &&!x.IsDeleted)
                .FirstOrDefault();

            if (column == null) throw new Exception(); //column dosent exist

            return column.Cards.Where(x => !x.IsDeleted).Count();
        }

        public int GetBoardId(int taskId)
        {
            return _dbContext.Cards
                .Include(x => x.Column)
                .FirstOrDefault(x => x.Id == taskId)
                .Column.BoardId;
        }

        public void Delete(int taskId)
        {
            var task = _dbContext.Cards.FirstOrDefault(x => x.Id == taskId);

            if (task == null) throw new Exception();

            task.IsDeleted = true;
            //todo: comments soft delete
            _dbContext.SaveChanges();
        }

        public void Update(Card card)
        {
            _dbContext.Entry(card).State = EntityState.Modified;

            _dbContext.SaveChanges();
        }
    }
}
