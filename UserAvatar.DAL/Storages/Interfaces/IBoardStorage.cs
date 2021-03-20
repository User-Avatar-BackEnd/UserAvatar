using System.Collections.Generic;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public  interface IBoardStorage
    {
        void Create(Board board);

        List<Board> ListBoardsById(int id);

        bool DoesUserHasBoards(int id);

        bool IsBoardExists(int id);

    }
}