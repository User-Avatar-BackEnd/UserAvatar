using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using System.Net;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Bll.Infrastructure;
using System.Net.Mime;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/boards/{boardId:int}/cards")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;

        public CardController(ICardService cardService, IMapper mapper, IApplicationUser applicationUser)
        {
            _cardService = cardService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }

        private int UserId => _applicationUser.Id;

        [HttpGet("{cardId:int}")]
        [ProducesResponseType(typeof(CardDetailedVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetByIdAsync(int boardId, int cardId)
        {
            var result = await _cardService.GetByIdAsync(boardId, cardId, UserId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            var cardVm = _mapper.Map<CardModel, CardDetailedVm>(result.Value);

            cardVm.Comments.ForEach(x => x.Editable = x.UserId == UserId);

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
            var result = await _cardService.CreateCardAsync(titleDto.Title, boardId, columnId, UserId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();
            if (result.Code != ResultCode.Success)
            {
                return Conflict(result.Code);
            }

            var cardVm = _mapper.Map<CardModel, CardShortVm>(result.Value);
            return Ok(cardVm);
        }

        [HttpPut("{cardId:int}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateCardAsync(int boardId, int cardId, UpdateCardDto updateCardDto)
        {
            var cardModel = _mapper.Map<UpdateCardDto, CardModel>(updateCardDto);
            cardModel.Id = cardId;

            var result = await _cardService.UpdateCardAsync(cardModel, boardId, UserId);

            return StatusCode(result);
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