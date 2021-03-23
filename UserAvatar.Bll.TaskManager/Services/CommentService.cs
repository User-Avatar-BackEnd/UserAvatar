﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
            await ValidateUserByCard(userId, cardId);

            var newComment = new Comment
            {
                CardId = cardId,
                UserId = userId,
                Text = text,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };

            await _commentStorage.Create(newComment);

            return _mapper.Map<Comment, CommentModel>(newComment);
        }

        public async Task<CommentModel> UpdateComment(int userId, int commentId, string text)
        {
            await ValidateUserByComment(userId, commentId);

            var thisComment = await _commentStorage.GetCommentByCommentId(commentId);
            thisComment.Text = text;

            await _commentStorage.UpdateComment(thisComment);

            return _mapper.Map<Comment, CommentModel>(thisComment);
        }

        public async Task<List<CommentModel>> GetComments(int userId, int cardId)
        {
            await ValidateUserByCard(userId, cardId);

            var commentList = await _commentStorage.GetAll(cardId);

            return _mapper.Map<List<Comment>, List<CommentModel>>(commentList);
        }

        public async Task DeleteComment(int userId, int commentId)
        {
            await ValidateUserByComment(userId, commentId);
            await _commentStorage.DeleteApparent(commentId);
        }

        private async Task ValidateUserByComment(int userId, int commentId)
        {
            var cardId = _commentStorage.GetTaskIdByCommentId(commentId);
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, _cardStorage.GetBoardId(cardId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }

        private async Task ValidateUserByCard(int userId, int cardId)
        {
            var isUserInThisBoard = await _boardStorage.IsUserBoardAsync(userId, _cardStorage.GetBoardId(cardId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }
    }
}