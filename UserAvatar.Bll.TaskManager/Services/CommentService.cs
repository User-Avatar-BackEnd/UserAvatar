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

        public async Task<CommentModel> CreateNewCommentAsync(int userId, int cardId, string text)
        {
            await ValidateUserByCardAsync(userId, cardId);

            var newComment = new Comment
            {
                CardId = cardId,
                UserId = userId,
                Text = text,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };

            await _commentStorage.CreateAsync(newComment);

            return _mapper.Map<Comment, CommentModel>(newComment);
        }

        public async Task<CommentModel> UpdateCommentAsync(int userId, int commentId, string text)
        {
            await ValidateUserByCommentAsync(userId, commentId);

            var thisComment = await _commentStorage.GetCommentByCommentIdAsync(commentId);
            thisComment.Text = text;

            await _commentStorage.UpdateCommentAsync(thisComment);

            return _mapper.Map<Comment, CommentModel>(thisComment);
        }

        public async Task<Result<List<CommentModel>>> GetCommentsAsync(int userId, int cardId)
        {
            await ValidateUserByCardAsync(userId, cardId);

            var commentList = await _commentStorage.GetAllAsync(cardId);

            return new Result<List<CommentModel>>(_mapper.Map<List<Comment>, List<CommentModel>>(commentList));
        }

        public async Task DeleteCommentAsync(int userId, int commentId)
        {
            await ValidateUserByCommentAsync(userId, commentId);
            await _commentStorage.DeleteApparentAsync(commentId);
        }

        private async Task ValidateUserByCommentAsync(int userId, int commentId)
        {
            var cardId = await _commentStorage.GetTaskIdByCommentIdAsync(commentId);
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, await _cardStorage.GetBoardIdAsync(cardId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }

        private async Task ValidateUserByCardAsync(int userId, int cardId)
        {
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, await _cardStorage.GetBoardIdAsync(cardId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }
    }
}