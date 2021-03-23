using System;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class CardService : ICardService
    {
        private readonly ICardStorage _cardStorage;
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public CardService(ICardStorage cardStorage, IMapper mapper, IBoardStorage boardStorage)
        {
            _cardStorage = cardStorage;
            _boardStorage = boardStorage;
            _mapper = mapper;
        }

        public CardModel CreateCard(string title, int columnId, int userId)
        {
            //var boardId = _cardStorage.GetBoardId(cardId);
            //if (_boardStorage.IsUsersBoard(userId, boardId)) return null;//isUserBoard

            if (string.IsNullOrEmpty(title)) return null;
            if (columnId < 1) return null;

            if (_cardStorage.GetCardsCountInColumn(columnId) > 100) throw new Exception();

            var card = new Card
            {
                Title = title,
                Description = "",
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsHidden = false,
                ColumnId = columnId
            };

            card = _cardStorage.Create(card);
            var cardModel = _mapper.Map<Card, CardModel>(card);
            return cardModel;
        }

        public async Task UpdateCardAsync(CardModel cardModel, int columnId, int? responsibleId, int userId)
        {
            var boardId = _cardStorage.GetBoardId(cardModel.Id);

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId)) throw new Exception();

            var card = _cardStorage.GetById(cardModel.Id);

            card.Title = card.Title;
            card.Description = cardModel.Description;
            card.ColumnId = columnId;
            card.ResponsibleId = responsibleId;
            card.IsHidden = cardModel.IsHidden;
            card.ModifiedAt = DateTime.UtcNow;
            card.Priority = cardModel.Priority;

            _cardStorage.Update(card);
        }

        public async Task<CardModel> GetByIdAsync(int cardId, int userId)
        {
            var boardId = _cardStorage.GetBoardId(cardId);
            if (!await _boardStorage.IsUserBoardAsync(userId, boardId)) throw new Exception();

            var card = _cardStorage.GetById(cardId);

            return card == null ? null : _mapper.Map<Card, CardModel>(card);
        }

        public async Task DeleteCard(int cardId, int userId)
        {
            if (_cardStorage.GetById(cardId) == null)
                return;
            var boardId = _cardStorage.GetBoardId(cardId);

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId)) throw new Exception();

            _cardStorage.Delete(cardId);
        }
    }
}
