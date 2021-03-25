using System;
using System.Collections.Concurrent;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class BoardChangesService : IBoardChangesService
    {
        private readonly ConcurrentDictionary<int, BoardLogModel> _boardsLogs;

        public BoardChangesService()
        {
            _boardsLogs = new ConcurrentDictionary<int, BoardLogModel>();
        }

        public void DoChange(int boardId, int userId)
        {
            var boardLogs = _boardsLogs.GetOrAdd(boardId, x => new BoardLogModel());
            boardLogs.AddChange(userId);
        }

        public bool HasChanges(int boardId, int userId, long ticks)
        {
            var boardLogs = _boardsLogs.GetOrAdd(boardId, x => new BoardLogModel());
            return boardLogs.HasChanges(userId, ticks);
        }
    }
}
