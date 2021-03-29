using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using System.Net;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Bll.Infrastructure;
using System.Net.Mime;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using UserAvatar.Api.Authentication;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/boards/{boardId:int}/cards")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        private readonly IHistoryService _historyService;

        public CardController(
            ICardService cardService,
            IMapper mapper,
            IApplicationUser applicationUser,
            IHistoryService historyService)
        {
            _cardService = cardService;
            _mapper = mapper;
            _applicationUser = applicationUser;
            _historyService = historyService;
        }

        private int UserId => _applicationUser.Id;

        [HttpGet("{cardId:int}")]
        [ProducesResponseType(typeof(CardDetailedVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetByIdAsync(int boardId, int cardId)
        {
            var result = await _cardService.GetCardByIdAsync(boardId, cardId, UserId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            var cardVm = _mapper.Map<CardModel, CardDetailedVm>(result.Value);

            cardVm.Comments.ForEach(x => x.Editable = x.UserId == UserId);
            cardVm.Comments = cardVm.Comments.OrderByDescending(x => x.CreatedAt).ToList();

            return Ok(cardVm);
        }

        [HttpPost]
        [Route("~/api/v1/boards/{boardId:int}/columns/{columnId:int}/cards")]
        [ProducesResponseType(typeof(CardShortVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<CardShortVm>> AddCardAsync(int boardId, int columnId, TitleDto titleDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _cardService.CreateCardAsync(titleDto.Title, boardId, columnId, UserId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();
            if (result.Code != ResultCode.Success)
            {
                return Conflict(result.Code);
            }

            var cardVm = _mapper.Map<CardModel, CardShortVm>(result.Value);

            await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

            return Ok(cardVm);
        }

        [HttpPut("{cardId:int}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateCardAsync(int boardId, int cardId, UpdateCardDto updateCardDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cardModel = _mapper.Map<UpdateCardDto, CardModel>(updateCardDto);
            cardModel.Id = cardId;

            var result = await _cardService.UpdateCardAsync(cardModel, boardId, UserId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

            return Ok();
        }

        [HttpDelete("{cardId:int}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteCardAsync(int boardId, int cardId)
        {
            var result = await _cardService.DeleteCardAsync(boardId, cardId, UserId);

            return StatusCode(result);
        }
    }
}

