using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ICommentStorage
    {
        Task<Comment> Create(Comment comment);

        Task DeleteApparent(int commentId);

        Task UpdateComment(Comment comment);

        Task RecursivelyDelete(IEnumerable<Comment> comments);

        Task<Comment> GetCommentByCommentId(int commentId);

        Task<List<Comment>> GetAll(int taskId);

        int GetTaskIdByCommentId(int commentId);
    }
}