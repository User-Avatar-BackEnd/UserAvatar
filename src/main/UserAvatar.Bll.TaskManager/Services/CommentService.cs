using System;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services;

public sealed class CommentService : ICommentService
{
    private readonly IBoardChangesService _boardChangesService;
    private readonly IBoardStorage _boardStorage;
    private readonly ICardStorage _cardStorage;
    private readonly ICommentStorage _commentStorage;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentStorage commentStorage,
        IBoardStorage boardStorage,
        ICardStorage cardStorage,
        IMapper mapper,
        IBoardChangesService boardChangesService)
    {
        _commentStorage = commentStorage;
        _boardStorage = boardStorage;
        _cardStorage = cardStorage;
        _mapper = mapper;
        _boardChangesService = boardChangesService;
    }

    public async Task<Result<CommentModel>> CreateNewCommentAsync(
        int userId, int boardId, int cardId, string text)
    {
        var validation = await ValidateUserCommentAsync(userId, boardId, cardId);
        if (validation != ResultCode.Success)
        {
            return new Result<CommentModel>(validation);
        }

        var newComment = new Comment
        {
            CardId = cardId,
            UserId = userId,
            Text = text,
            CreatedAt = DateTimeOffset.Now,
            ModifiedAt = DateTimeOffset.Now,
        };

        // TODO: It returns the comment, but shouldn't , let's return nothing
        var comment = await _commentStorage.CreateAsync(newComment);

        _boardChangesService.DoChange(boardId, userId);

        return new Result<CommentModel>(_mapper.Map<Comment, CommentModel>(comment));
    }

    public async Task<Result<CommentModel>> UpdateCommentAsync(
        int userId, int boardId, int cardId, int commentId, string text)
    {
        var validation = await ValidateUserCommentAsync(userId, boardId, cardId, commentId);
        if (validation != ResultCode.Success)
        {
            return new Result<CommentModel>(validation);
        }

        var thisComment = await _commentStorage.GetCommentByCommentIdAsync(commentId);
        thisComment.Text = text;

        await _commentStorage.UpdateCommentAsync(thisComment);

        return new Result<CommentModel>(_mapper.Map<Comment, CommentModel>(thisComment));
    }

    /*public async Task<Result<List<CommentModel>>> GetCommentsAsync(int userId, int cardId)
    {
        await ValidateUserByCardAsync(userId, cardId);

        var commentList = await _commentStorage.GetAllAsync(cardId);

        return new Result<List<CommentModel>>(_mapper.Map<List<Comment>, List<CommentModel>>(commentList));
    }*/

    public async Task<int> DeleteCommentAsync(int userId, int boardId, int cardId, int commentId)
    {
        var validation = await ValidateUserCommentAsync(userId, boardId, cardId, commentId);
        if (validation != ResultCode.Success)
        {
            return validation;
        }

        await _commentStorage.DeleteApparentAsync(commentId);

        _boardChangesService.DoChange(boardId, userId);

        return ResultCode.Success;
    }

    private async Task<int> ValidateUserCommentAsync(int userId, int boardId, int cardId, int? commentId = null)
    {
        if (!await _boardStorage.IsBoardExistAsync(boardId))
        {
            return ResultCode.NotFound;
        }

        if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
        {
            return ResultCode.Forbidden;
        }

        if (!await _boardStorage.IsBoardCardAsync(boardId, cardId))
        {
            return ResultCode.NotFound;
        }

        if (commentId != null)
        {
            if (!await _cardStorage.IsCardCommentAsync(cardId, (int)commentId))
            {
                return ResultCode.NotFound;
            }
        }

        return ResultCode.Success;
    }
}
