using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
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
        private readonly IBoardChangesService _boardChangesService;

        public CardService(
            ICardStorage cardStorage,
            IMapper mapper,
            IBoardStorage boardStorage,
            IColumnStorage columnStorage,
            IOptions<LimitationOptions> limitations,
            IBoardChangesService boardChangesService)
        {
            _cardStorage = cardStorage;
            _boardStorage = boardStorage;
            _mapper = mapper;
            _columnStorage = columnStorage;
            _limitations = limitations.Value;
            _boardChangesService = boardChangesService;
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

            if (await _cardStorage.GetCardsCountInColumnAsync(columnId) >= _limitations.MaxCardCount)
            {
                return new Result<CardModel>(ResultCode.MaxCardCount);
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

            var board = await _boardStorage.GetBoardAsync(boardId);
            var eventType = board.OwnerId == userId ?
                EventType.CreateCardOnOwnBoard :
                EventType.CreateCardOnAlienBoard;

            _boardChangesService.DoChange(boardId, userId);

            return new Result<CardModel>(cardModel, eventType);
        }

        public async Task<Result<bool>> UpdateCardAsync(CardModel cardModel, 
            int boardId, int userId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<bool>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsBoardColumnAsync(boardId, cardModel.ColumnId))
            {
                return new Result<bool>(ResultCode.Forbidden);
            }

            var card = await _cardStorage.GetByIdAsync(cardModel.Id);
            
            if (card == null)
            {
                return new Result<bool>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<bool>(ResultCode.Forbidden);
            }

            var statusChanged = card.ColumnId != cardModel.ColumnId;
                
            card.Title = cardModel.Title;
            card.Description = cardModel.Description;
            card.ColumnId = cardModel.ColumnId;
            card.ResponsibleId = cardModel.ResponsibleId;
            card.IsHidden = cardModel.IsHidden;
            card.ModifiedAt = DateTimeOffset.UtcNow;
            card.Priority = cardModel.Priority;

            await _cardStorage.UpdateAsync(card);

            _boardChangesService.DoChange(boardId, userId);

            if (!statusChanged) return new Result<bool>(true);

            var board = await _boardStorage.GetBoardAsync(boardId);
            var eventType = board.OwnerId == userId ?
                EventType.ChangeCardStatusOnOwnBoard:
                EventType.ChangeCardStatusOnAlienBoard;

            return new Result<bool>(true,eventType);
        }

        public async Task<Result<CardModel>> GetCardByIdAsync(
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

            _boardChangesService.DoChange(boardId, userId);

            return ResultCode.Success;
        }
    }
}
