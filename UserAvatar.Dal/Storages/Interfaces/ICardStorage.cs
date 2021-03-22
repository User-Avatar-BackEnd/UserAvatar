using System;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ICardStorage
    {
        public Card GetById(int id);

        public Card Create(Card card);

        public int GetCardsCountInColumn(int columnId);

        public int GetBoardId(int taskId);

        public void Delete(int taskId);

        public void Update(Card card);
    }
}
