using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentStorage _commentStorage;
        private readonly IBoardStorage _boardStorage;
        private readonly ICardStorage _cardStorage;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentStorage commentStorage,
            IBoardStorage boardStorage,
            ICardStorage cardStorage,
            IMapper mapper)
        {
            _commentStorage = commentStorage;
            _boardStorage = boardStorage;
            _cardStorage = cardStorage;
            _mapper = mapper;
        }

        public async Task<Result<CommentModel>> CreateNewCommentAsync(int userId, int boardId, int cardId, string text)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<CommentModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<CommentModel>(ResultCode.Forbidden);
            }

            if (!await _boardStorage.IsBoardCard(boardId, cardId))
            {
                return new Result<CommentModel>(ResultCode.Forbidden);
            }

            //await ValidateUserByCardAsync(userId, cardId);

            var newComment = new Comment
            {
                CardId = cardId,
                UserId = userId,
                Text = text,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            // TODO: It returns the comment, but shouldn't , let's return nothing
            var comment = await _commentStorage.CreateAsync(newComment);

            return new Result<CommentModel>(_mapper.Map<Comment, CommentModel>(comment));
        }

        public async Task<Result<CommentModel>> UpdateCommentAsync(int userId, int boardId, int cardId, int commentId, string text)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return new Result<CommentModel>(ResultCode.NotFound);
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return new Result<CommentModel>(ResultCode.Forbidden);
            }

            if (!await _boardStorage.IsBoardCard(boardId, cardId))
            {
                return new Result<CommentModel>(ResultCode.Forbidden);
            }

            if (!await _cardStorage.IsCardComment(cardId, commentId))
            {
                return new Result<CommentModel>(ResultCode.Forbidden);
            }

            // await ValidateUserByCommentAsync(userId, commentId);

            var thisComment = await _commentStorage.GetCommentByCommentIdAsync(commentId);
            thisComment.Text = text;

            await _commentStorage.UpdateCommentAsync(thisComment);

            return new Result<CommentModel>(_mapper.Map<Comment, CommentModel>(thisComment));
        }

        //public async Task<Result<List<CommentModel>>> GetCommentsAsync(int userId, int cardId)
        //{
        //    if(!await ValidateUserByCardAsync(userId, cardId))
        //        return new Result<List<CommentModel>>(ResultCode.Forbidden);

        //    var commentList = await _commentStorage.GetAllAsync(cardId);

        //    return new Result<List<CommentModel>>(_mapper.Map<List<Comment>, List<CommentModel>>(commentList));
        //}

        public async Task<int> DeleteCommentAsync(int userId, int boardId, int cardId, int commentId)
        {
            if (!await _boardStorage.IsBoardExistAsync(boardId))
            {
                return ResultCode.NotFound;
            }

            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
            {
                return ResultCode.Forbidden;
            }

            if (!await _boardStorage.IsBoardCard(boardId, cardId))
            {
                return ResultCode.Forbidden;
            }

            if (!await _cardStorage.IsCardComment(cardId, commentId))
            {
                return ResultCode.Forbidden;
            }

            // await ValidateUserByCommentAsync(userId, commentId);
            await _commentStorage.DeleteApparentAsync(commentId);

            return ResultCode.Success;
        }

        private async Task ValidateUserByCommentAsync(int userId, int commentId)
        {
            var cardId = await _commentStorage.GetTaskIdByCommentIdAsync(commentId);
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, await _cardStorage.GetBoardIdAsync(cardId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }

        private async Task<bool> ValidateUserByCardAsync(int userId, int cardId)
        {
            var thisBoard = await _cardStorage.GetBoardIdAsync(cardId);
            if (thisBoard == 0)
                return false;
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, thisBoard);
            return isUserInThisBoard;
        }
    }
}