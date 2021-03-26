using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IBoardService
    {
        Task<Result<IEnumerable<BoardModel>>> GetAllBoardsAsync(int userId);

        Task<Result<BoardModel>> CreateBoardAsync(int userId, string title);

        Task<Result<BoardModel>> GetBoardAsync(int userId, int boardId);

        Task<int> RenameBoardAsync(int userId, int boardId, string title);

        Task<int> DeleteBoardAsync(int userId, int boardId);

        Task<bool> IsUserBoardAsync(int userId, int boardId);

        Task<int> DeleteMemberFromBoardAsync(int userId, int toDeleteUserId, int boardId);
    }
}