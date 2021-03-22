﻿using System.Collections.Generic;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface IBoardStorage
    {
        System.Threading.Tasks.Task CreateBoardAsync(Board board);

        System.Threading.Tasks.Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);

        System.Threading.Tasks.Task<Board> GetBoardAsync(int userId, int boardId);

        System.Threading.Tasks.Task UpdateAsync(int userId, Board board);

        System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId);

        bool DoesUserHasBoard(int userId, string title);

        bool IsOwnerBoard(int userId, int boardId);

        bool IsUserBoard(int userId, int boardId);
    }
}