using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces;

public interface ICommentService
{
    Task<Result<CommentModel>> CreateNewCommentAsync(int userId, int boardId, int cardId, string text);

    Task<Result<CommentModel>> UpdateCommentAsync(int userId, int boardId, int cardId, int commentId, string text);

    Task<int> DeleteCommentAsync(int userId, int boardId, int cardId, int commentId);
}
