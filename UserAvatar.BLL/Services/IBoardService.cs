using System.Collections.Generic;
using UserAvatar.BLL.Models;

namespace UserAvatar.BLL.Services
{
    public interface IBoardService
    {
        System.Threading.Tasks.Task<IEnumerable<BoardModel>> GetAllBoards(int userId);

        System.Threading.Tasks.Task<bool> CreateBoard(int userId, string title);

        System.Threading.Tasks.Task<BoardModel> GetBoard(int userId, int boardId);

        System.Threading.Tasks.Task<bool> RenameBoard(int userId, int boardId, string title);

        System.Threading.Tasks.Task<bool> DeleteBoard(int userId, int boardId);
    }
}