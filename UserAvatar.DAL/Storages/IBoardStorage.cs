using System.Collections.Generic;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public  interface IBoardStorage
    {
        System.Threading.Tasks.Task<bool> CreateBoard(int userId, Board board);

        System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoards(int userId);

        System.Threading.Tasks.Task<Board> GetBoard(int userId, int boardId);

        System.Threading.Tasks.Task Update(Board board);

        System.Threading.Tasks.Task<bool> DeleteBoard(int userId, int boardId);

        bool DoesUserHasBoard(int userId, string title);

        bool IsUsersBoard(int userId, int boardId);
    }
}