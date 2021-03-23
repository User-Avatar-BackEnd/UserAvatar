using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentModel> CreateNewCommentAsync(int userId, int cardId, string text);

        Task<CommentModel> UpdateComment(int userId, int commentId, string text);

        Task<List<CommentModel>> GetComments(int userId, int cardId);

        Task DeleteComment(int userId, int commentId);
    }
}