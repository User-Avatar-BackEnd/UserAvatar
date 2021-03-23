using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.ViewModel;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/card")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;

        public CardController(ICardService cardService, IMapper mapper)
        {
            _cardService = cardService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var card = await _cardService.GetByIdAsync(id, userId);
            if (card == null) BadRequest();
            
            var cardVm = _mapper.Map<CardModel, CardDetailedVm>(card);

            cardVm.Comments.ForEach(x => x.Editable = x.UserId == userId);

            return Ok(cardVm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCardAsync(CreateCardDto createCardDto)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var card = await _cardService.CreateCardAsync(createCardDto.Title, createCardDto.ColumnId, userId);

            var cardVm = _mapper.Map<CardModel, CardShortVm>(card);

            return Ok(cardVm);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCardAsync(UpdateCardDto updateCardDto)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var cardModel = _mapper.Map<UpdateCardDto, CardModel>(updateCardDto);

            await _cardService.UpdateCardAsync(cardModel, updateCardDto.ColumnId, updateCardDto.ResponsibleId, userId);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCardAsync(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            await _cardService.DeleteCardAsync(id, userId);

            return Ok();
        }
    }
}