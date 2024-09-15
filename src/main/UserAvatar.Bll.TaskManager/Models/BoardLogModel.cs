using System;
using System.Collections.Concurrent;
using System.Linq;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.TaskManager.Models;

public sealed class BoardLogModel
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly ConcurrentQueue<Log> _logs;

    private readonly long _lifeTime = TimeSpan.FromMinutes(1).Ticks;

    public BoardLogModel(IDateTimeProvider dateTimeProvider)
    {
        _logs = new ConcurrentQueue<Log>();
        _dateTimeProvider = dateTimeProvider;
    }

    public void AddChange(int userId)
    {
        Clear();
        _logs.Enqueue(new Log(userId, _dateTimeProvider.DateTimeUtcNowTicks()));
    }

    public bool HasChanges(int userId, long lastCheck)
    {
        Clear();
        if (_dateTimeProvider.DateTimeUtcNowTicks() - lastCheck > _lifeTime)
        {
            return true;
        }

        var logsSnapshot = _logs.ToArray().Reverse();
        foreach (var log in logsSnapshot)
        {
            if (log.Ticks >= lastCheck && userId != log.UserId)
            {
                return true;
            }

            if (log.Ticks < lastCheck)
            {
                return false;
            }
        }

        return false;
    }

    private void Clear()
    {
        Log log;

        while (_logs.TryPeek(out log))
        {
            if (_dateTimeProvider.DateTimeUtcNowTicks() - log.Ticks < _lifeTime)
            {
                break;
            }

            _logs.TryDequeue(out log);
        }
    }

    private readonly struct Log
    {
        public Log(int userId, long ticks)
        {
            UserId = userId;
            Ticks = ticks;
        }

        public int UserId { get; }

        public long Ticks { get; }
    }
}
