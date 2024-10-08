﻿using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers;

/// <summary>
///     Card controlelr
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/boards/{boardId:int}/cards")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class CardController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly ICardService _cardService;
    private readonly IHistoryService _historyService;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="cardService">card service</param>
    /// <param name="mapper">automapper</param>
    /// <param name="applicationUser">this user id</param>
    /// <param name="historyService">history service</param>
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

    /// <summary>
    ///     Gets card by its id
    /// </summary>
    /// <param name="boardId">board where card is</param>
    /// <param name="cardId">card id</param>
    /// <returns></returns>
    [HttpGet("{cardId:int}")]
    [ProducesResponseType(typeof(CardDetailedVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetByIdAsync(int boardId, int cardId)
    {
        var result = await _cardService.GetCardByIdAsync(boardId, cardId, UserId);

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        var cardVm = _mapper.Map<CardModel, CardDetailedVm>(result.Value);

        cardVm.Comments.ForEach(x => x.Editable = x.UserId == UserId);
        cardVm.Comments = cardVm.Comments.OrderByDescending(x => x.CreatedAt).ToList();

        return Ok(cardVm);
    }

    /// <summary>
    ///     Adding a card to the board
    /// </summary>
    /// <param name="boardId">board to be added id</param>
    /// <param name="columnId">column to be added id</param>
    /// <param name="titleDto">title of the card</param>
    /// <returns></returns>
    [HttpPost]
    [Route("~/api/v1/boards/{boardId:int}/columns/{columnId:int}/cards")]
    [ProducesResponseType(typeof(CardShortVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<CardShortVm>> AddCardAsync(int boardId, int columnId, TitleDto titleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _cardService.CreateCardAsync(titleDto.Title, boardId, columnId, UserId);

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        if (result.Code != ResultCode.Success)
        {
            return Conflict(result.Code);
        }

        var cardVm = _mapper.Map<CardModel, CardShortVm>(result.Value);

        await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

        return Ok(cardVm);
    }

    /// <summary>
    ///     Updates card
    /// </summary>
    /// <param name="boardId">board id where card is situated</param>
    /// <param name="cardId">card to be updated id</param>
    /// <param name="updateCardDto">update data</param>
    /// <returns></returns>
    [HttpPut("{cardId:int}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateCardAsync(int boardId, int cardId, UpdateCardDto updateCardDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cardModel = _mapper.Map<UpdateCardDto, CardModel>(updateCardDto);
        cardModel.Id = cardId;

        var result = await _cardService.UpdateCardAsync(cardModel, boardId, UserId);

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

        return Ok();
    }

    /// <summary>
    ///     Soft deletes card
    /// </summary>
    /// <param name="boardId">board id where card is situated in</param>
    /// <param name="cardId">this card id</param>
    /// <returns></returns>
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
