using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IBoardStorage
    {
        Task CreateBoardAsync(Board board);

        Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);

        Task<Board> GetBoardAsync(int userId, int boardId);

        Task UpdateAsync(int userId, Board board);

        Task DeleteBoardAsync(int userId, int boardId);

        Task<bool> DoesUserHasBoardAsync(int userId, string title);

        Task<bool> IsOwnerBoardAsync(int userId, int boardId);

        Task<bool> IsUserBoardAsync(int userId, int boardId);
    }
}