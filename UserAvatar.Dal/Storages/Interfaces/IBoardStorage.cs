﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IBoardStorage
    {
        Task CreateBoardAsync(Board board);
        Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);
        Task<int> CountAllBoardsAsync(int userId);
        Task<Board> GetBoardAsync(int boardId);
        Task UpdateAsync(int userId, Board board);
        Task UpdateMemberAsync(Member member);
        /// <summary>
        /// Checks if user is member in this board
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boardId"></param>
        /// <returns></returns>
        Task<bool> IsUserBoardAsync(int userId, int boardId);
        Task DeleteRecurrentlyBoardAsync(int boardId);
        Task<Member> GetMemberByIdAsync(int userId, int boardId);
        /// <summary>
        /// Checks if user is owner of this board
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boardId"></param>
        /// <returns></returns>
        Task<bool> IsOwnerBoardAsync(int userId, int boardId);
        Task<bool> IsBoardExistAsync(int boardId);
        /// <summary>
        /// Adds member to the board!
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        Task AddAsMemberAsync(Member member);
        Task<bool> IsBoardColumn(int boardId, int columnId);
        Task<bool> IsBoardCard(int boardId, int cardId);
    }
}