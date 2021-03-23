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

        public async Task<Comment> CreateAsync(Comment comment)
        {
            await _userAvatarContext.AddAsync(comment);
            await _userAvatarContext.SaveChangesAsync();

            return await _userAvatarContext.Comments
                .Include(x => x.User)
                .FirstAsync(x => x.Id == comment.Id);
        }

        public async Task DeleteApparentAsync(int commentId)
        {
            var thisComment = await GetCommentByCommentIdAsync(commentId);

            thisComment.IsDeleted = true;
            _userAvatarContext.Comments.Update(thisComment);
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetAllAsync(int cardId)
        {
            //async?
            //return Task.FromResult(_userAvatarContext.Comments
            //    .Where(x => x.CardId == cardId)
            //    .OrderBy(x=> x.ModifiedAt)
            //    .ToList());
            return await _userAvatarContext.Comments
                .Where(x => x.CardId == cardId)
                .OrderBy(x => x.ModifiedAt)
                .ToListAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            var previousComment = await _userAvatarContext.Comments.FindAsync(comment.Id);
            previousComment.ModifiedAt = DateTime.Now;
            previousComment.Text = comment.Text;

            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task RecursivelyDeleteAsync(IEnumerable<Comment> comments)
        {
            foreach (var comment in comments)
            {
                comment.IsDeleted = true;
            }
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByCommentIdAsync(int commentId)
        {
            return await _userAvatarContext.Comments.FindAsync(commentId);
        }

        public async Task<int> GetTaskIdByCommentIdAsync(int commentId)
        {
            var comment = await _userAvatarContext.Comments
                 .Include(x => x.Card)
                 .FirstAsync(x => x.Id == commentId);

            return comment.CardId;
        }
    }
}