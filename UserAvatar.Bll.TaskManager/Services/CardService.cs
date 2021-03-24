using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
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
        private readonly IColumnStorage _columnStorage;
        private readonly LimitationOptions _limitations;
        private readonly IMapper _mapper;

        public CardService(
            ICardStorage cardStorage,
            IMapper mapper,
            IBoardStorage boardStorage,
            IColumnStorage columnStorage,
            IOptions<LimitationOptions> limitations)
        {
            _cardStorage = cardStorage;
            _boardStorage = boardStorage;
            _mapper = mapper;
            _columnStorage = columnStorage;
            _limitations = limitations.Value;
        }

        public async Task<Result<CardModel>> CreateCardAsync(string title, 
            int boardId, int columnId, int userId)
        {
            var column = await _columnStorage.GetColumnByIdAsync(columnId);
            
            if(column==null || column.BoardId != boardId)
            {
                return new Result<CardModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<CardModel>(ResultCode.Forbidden);
            }

            if (await _cardStorage.GetCardsCountInColumnAsync(columnId) > _limitations.MaxCardCount)
            {
                return new Result<CardModel>(ResultCode.MaxColumnCount);
            }

            var card = new Card
            {
                Title = title,
                Description = "",
                OwnerId = userId,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow,
                ColumnId = columnId,
                ModifiedBy = userId
            };

            card = await _cardStorage.CreateAsync(card);
            var cardModel = _mapper.Map<Card, CardModel>(card);

            return new Result<CardModel>(cardModel);
        }

        public async Task<int> UpdateCardAsync(CardModel cardModel, 
            int boardId, int userId)
        {
            var card = await _cardStorage.GetByIdAsync(cardModel.Id);
            
            if (card == null)
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            card.Title = card.Title;
            card.Description = cardModel.Description;
            card.ColumnId = cardModel.ColumnId;
            card.ResponsibleId = cardModel.ResponsibleId;
            card.IsHidden = cardModel.IsHidden;
            card.ModifiedAt = DateTimeOffset.UtcNow;
            card.Priority = cardModel.Priority;

            await _cardStorage.UpdateAsync(card);
            return ResultCode.Success;
        }

        public async Task<Result<CardModel>> GetByIdAsync(
            int boardId, int cardId, int userId)
        {
            var card = await _cardStorage.GetByIdAsync(cardId);
            if (card == null)
            {
                return new Result<CardModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<CardModel>(ResultCode.Forbidden);
            }

            return new Result<CardModel>(_mapper.Map<Card, CardModel>(card));
        }

        public async Task<int> DeleteCardAsync(
            int boardId, int cardId, int userId)
        {
            var card = await _cardStorage.GetByIdAsync(cardId);
            if (card == null)
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            await _cardStorage.DeleteAsync(cardId);
            return ResultCode.Success;
        }
    }
}