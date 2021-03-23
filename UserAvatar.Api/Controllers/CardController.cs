using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/boards/{boardId:int}/columns/{columnId:int}/cards")]
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
        public async Task<IActionResult> GetByIdAsync(int boardId, int columnId, int cardId)
        {
            var card = await _cardService.GetByIdAsync(cardId, UserId);
            if (card == null) BadRequest();
            
            var cardVm = _mapper.Map<CardModel, CardDetailedVm>(card);

            cardVm.Comments.ForEach(x => x.Editable = x.UserId == UserId);

            return Ok(cardVm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCardAsync(int boardId, int columnId, TitleDto titleDto)
        {
            var card = await _cardService.CreateCardAsync(titleDto.Title, columnId, UserId);

            var cardVm = _mapper.Map<CardModel, CardShortVm>(card);

            return Ok(cardVm);
        }

        [HttpPut("{cardId:int}")]
        public async Task<IActionResult> UpdateCardAsync(int boardId, int columnId, int cardId, UpdateCardDto updateCardDto)
        {
            var cardModel = _mapper.Map<UpdateCardDto, CardModel>(updateCardDto);

            await _cardService.UpdateCardAsync(cardModel, columnId, updateCardDto.ResponsibleId, UserId);

            return Ok();
        }

        [HttpDelete("{cardId:int}")]
        public async Task<IActionResult> DeleteCardAsync(int boardId, int columnId, int cardId)
        {
            await _cardService.DeleteCardAsync(cardId, UserId);

            return Ok();
        }
    }
}