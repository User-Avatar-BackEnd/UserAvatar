using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ICommentStorage
    {
        Task<Comment> CreateAsync(Comment comment);

        Task DeleteApparentAsync(int commentId);

        Task UpdateCommentAsync(Comment comment);

        Task RecursivelyDeleteAsync(IEnumerable<Comment> comments);

        Task<Comment> GetCommentByCommentIdAsync(int commentId);

        Task<List<Comment>> GetAllAsync(int cardId);

        Task<int> GetTaskIdByCommentIdAsync(int commentId);
    }
}