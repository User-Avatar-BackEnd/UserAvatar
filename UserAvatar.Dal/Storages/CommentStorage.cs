using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class CommentStorage : ICommentStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public CommentStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public async Task<Comment> Create(Comment comment)
        {
            await _userAvatarContext.AddAsync(comment);
            await _userAvatarContext.SaveChangesAsync();

            return _userAvatarContext.Comments
                .Include(x => x.User)
                .First(x => x.Id == comment.Id);
        }

        public async Task DeleteApparent(int commentId)
        {
            var thisComment = await GetCommentByCommentId(commentId);

            thisComment.IsDeleted = true;
            _userAvatarContext.Comments.Update(thisComment);
            await _userAvatarContext.SaveChangesAsync();
        }

        public Task<List<Comment>> GetAll(int taskId)
        {
            //async?
            return Task.FromResult(_userAvatarContext.Comments
                .Where(x => x.CardId == taskId)
                .OrderBy(x=> x.ModifiedAt)
                .ToList());
        }

        public async Task UpdateComment(Comment comment)
        {
            var previousComment = await _userAvatarContext.Comments.FindAsync(comment.Id);
            previousComment.ModifiedAt = DateTime.Now;
            previousComment.Text = comment.Text;

            await _userAvatarContext.SaveChangesAsync();

        }

        public async Task RecursivelyDelete(IEnumerable<Comment> comments)
        {
            foreach (var comment in comments)
            {
                comment.IsDeleted = true;
            }
            await _userAvatarContext.SaveChangesAsync();
        }
        public async Task<Comment> GetCommentByCommentId(int commentId)
        {
            return await _userAvatarContext.Comments.FindAsync(commentId);
        }

        public int GetTaskIdByCommentId(int commentId)
        {
            return _userAvatarContext.Comments
                .Include(x => x.Card)
                .First(x => x.Id == commentId).CardId;
        }
    }
}