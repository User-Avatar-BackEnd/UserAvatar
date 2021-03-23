using System;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
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

            var task = new Card
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

            task = _cardStorage.Create(task);
            var taskModel = _mapper.Map<Card, CardModel>(task);
            return taskModel;
        }

        public async Task UpdateCardAsync(CardModel cardModel, int columnId, int? responsibleId, int userId)
        {
            var boardId = _cardStorage.GetBoardId(cardModel.Id);

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId)) throw new Exception();

            var task = _cardStorage.GetById(cardModel.Id);

            task.Title = task.Title;
            task.Description = cardModel.Description;
            task.ColumnId = columnId;
            task.ResponsibleId = responsibleId;
            task.IsHidden = cardModel.IsHidden;
            task.ModifiedAt = DateTime.UtcNow;
            task.Priority = cardModel.Priority;

            _cardStorage.Update(task);
        }

        public async Task<CardModel> GetByIdAsync(int taskId, int userId)
        {
            var boardId = _cardStorage.GetBoardId(taskId);
            if (!await _boardStorage.IsUserBoardAsync(userId,boardId)) throw new Exception();

            var task = _cardStorage.GetById(taskId);

            return task == null ? null : _mapper.Map<Card, CardModel>(task);
        }

        public async Task DeleteCard(int taskId, int userId)
        {
            if (_cardStorage.GetById(taskId) == null)
                return;
            var boardId = _cardStorage.GetBoardId(taskId);

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId)) throw new Exception();

            _cardStorage.Delete(taskId);
        }
    }
}
