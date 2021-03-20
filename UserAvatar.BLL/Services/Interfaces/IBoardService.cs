using System.Collections.Generic;
using UserAvatar.BLL.DTOs;

namespace UserAvatar.BLL.Services.Interfaces
{
    public interface IBoardService
    {
        List<BoardsDto> GetAllBoardsById(int id);

        bool CreateBoard(int id, string name);
    }
}