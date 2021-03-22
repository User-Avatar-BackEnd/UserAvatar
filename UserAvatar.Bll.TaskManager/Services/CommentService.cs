using System;
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
        private readonly ICardStorage _taskStorage;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentStorage commentStorage,
            IBoardStorage boardStorage,
            ICardStorage taskStorage,
            IMapper mapper)
        {
            _commentStorage = commentStorage;
            _boardStorage = boardStorage;
            _taskStorage = taskStorage;
            _mapper = mapper;
        }

        public async Task<CommentModel> CreateNewCommentAsync(int userId, int taskId, string text)
        {
            ValidateUserByTask(userId,taskId);
            
            var newComment = new Comment
            {
                CardId = taskId,
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
            ValidateUserByComment(userId,commentId);
            
            var thisComment = await _commentStorage.GetCommentByCommentId(commentId);
            thisComment.Text = text;

            await _commentStorage.UpdateComment(thisComment);
            
            return _mapper.Map<Comment, CommentModel>(thisComment);
        }

        public async Task<List<CommentModel>> GetComments(int userId,int taskId)
        {
            ValidateUserByTask(userId,taskId);

            var commentList = await _commentStorage.GetAll(taskId);
            
            return _mapper.Map<List<Comment>, List<CommentModel>>(commentList);
        }

        public async Task DeleteComment(int userId, int commentId)
        {
            ValidateUserByComment(userId, commentId);
            await _commentStorage.DeleteApparent(commentId);
        }
        private void ValidateUserByComment(int userId, int commentId)
        {
            var taskId = _commentStorage.GetTaskIdByCommentId(commentId);
            var isUserInThisBoard = _boardStorage.IsUserBoard(userId, _taskStorage.GetBoardId(taskId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }
        private void ValidateUserByTask(int userId, int taskId)
        {
            var isUserInThisBoard = _boardStorage.IsUserBoard(userId, _taskStorage.GetBoardId(taskId));
            if (!isUserInThisBoard)
                throw new Exception($"You {userId} are not allowed to do this!");
        }
    }
}