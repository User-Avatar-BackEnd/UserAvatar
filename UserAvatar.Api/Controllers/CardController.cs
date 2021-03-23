using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetById(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var card = await _cardService.GetByIdAsync(id, userId);
            if (card == null) BadRequest();
            
            var cardDto = _mapper.Map<CardModel, CardDetailedDto>(card);

            cardDto.Comments.ForEach(x => x.Editable = x.UserId == userId);

            return Ok(cardDto);
        }

        [HttpPost]
        public IActionResult AddCard(CreateCardRequest request)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var card = _cardService.CreateCard(request.Title, request.ColumnId, userId);

            var cardDto = _mapper.Map<CardModel, CardShortDto>(card);

            return Ok(cardDto);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCard(UpdateCardRequest request)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var cardModel = _mapper.Map<UpdateCardRequest, CardModel>(request);

            await _cardService.UpdateCardAsync(cardModel, request.ColumnId, request.ResponsibleId, userId);

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteCard(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            _cardService.DeleteCard(id, userId);

            return Ok();
        }
    }
}