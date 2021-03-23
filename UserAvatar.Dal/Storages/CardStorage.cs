﻿using System;
using System.Linq;
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
                .FirstOrDefault(x => x.Id == columnId &&!x.IsDeleted);
            //!x.IsDeleted???

            if (column == null) throw new Exception(); //column dosent exist

            return column.Cards.Where(x => !x.IsDeleted).Count();
        }

        public int GetBoardId(int cardId)
        {
            return _dbContext.Cards
                .Include(x => x.Column)
                .FirstOrDefault(x => x.Id == cardId)
                .Column.BoardId;
        }

        public void Delete(int cardId)
        {
            var card = _dbContext.Cards.FirstOrDefault(x => x.Id == cardId);

            if (card == null) throw new Exception();

            card.IsDeleted = true;
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
