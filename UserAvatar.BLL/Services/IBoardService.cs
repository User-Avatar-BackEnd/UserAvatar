using System.Collections.Generic;
using UserAvatar.BLL.Models;

namespace UserAvatar.BLL.Services
{
    public interface IBoardService
    {
        List<BoardModel> GetAllBoardsById(int id);

        bool CreateBoard(int id, string name);
    }
}