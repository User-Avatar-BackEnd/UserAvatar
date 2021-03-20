using System.Collections.Generic;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public  interface IBoardStorage
    {
        void Create(Board board);

        IEnumerable<Board> ListBoardsById(int id);

        bool DoesUserHasBoards(int id);

        bool IsBoardExists(int id);

    }
}