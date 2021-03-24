using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IBoardStorage
    {
        Task CreateBoardAsync(Board board);

        Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);

        Task<Board> GetBoardAsync(int boardId);

        Task UpdateAsync(int userId, Board board);

        Task DeleteBoardAsync(int userId, int boardId);

        Task<bool> IsOwnerBoardAsync(int userId, int boardId);

        Task<bool> IsUserBoardAsync(int userId, int boardId);

        Task<bool> IsBoardExistAsync(int boardId);
        Task AddAsMemberAsync(int userId, int boardId);
        Task<bool> IsBoardColumn(int boardId, int columnId);
        
        Task<bool> IsBoardCard(int boardId, int cardId);
    }
}