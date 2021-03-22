using System.Collections.Generic;
using UserAvatar.Bll.Models;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface IBoardService
    {
        System.Threading.Tasks.Task<IEnumerable<BoardModel>> GetAllBoardsAsync(int userId);

        System.Threading.Tasks.Task CreateBoardAsync(int userId, string title);

        System.Threading.Tasks.Task<BoardModel> GetBoardAsync(int userId, int boardId);

        System.Threading.Tasks.Task RenameBoardAsync(int userId, int boardId, string title);

        System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId);
    }
}