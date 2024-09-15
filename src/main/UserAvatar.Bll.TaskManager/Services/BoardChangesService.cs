using System.Collections.Concurrent;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services;

public sealed class BoardChangesService : IBoardChangesService
{
    private readonly ConcurrentDictionary<int, BoardLogModel> _boardsLogs;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BoardChangesService(IDateTimeProvider dateTimeProvider)
    {
        _boardsLogs = new ConcurrentDictionary<int, BoardLogModel>();
        _dateTimeProvider = dateTimeProvider;
    }

    public void DoChange(int boardId, int userId)
    {
        var boardLogs = _boardsLogs.GetOrAdd(boardId, x => new BoardLogModel(_dateTimeProvider));
        boardLogs.AddChange(userId);
    }

    public bool HasChanges(int boardId, int userId, long ticks)
    {
        var boardLogs = _boardsLogs.GetOrAdd(boardId, x => new BoardLogModel(_dateTimeProvider));
        return boardLogs.HasChanges(userId, ticks);
    }
}
