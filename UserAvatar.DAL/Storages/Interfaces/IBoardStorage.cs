using System.Collections.Generic;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public  interface IBoardStorage
    {
        System.Threading.Tasks.Task<bool> CreateBoardAsync(int userId, Board board);

        System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);

        System.Threading.Tasks.Task<Board> GetBoardAsync(int userId, int boardId);

        System.Threading.Tasks.Task UpdateAsync(Board board);

        System.Threading.Tasks.Task<bool> DeleteBoardAsync(int userId, int boardId);

        bool DoesUserHasBoard(int userId, string title);

        bool IsUsersBoard(int userId, int boardId);
    }
}