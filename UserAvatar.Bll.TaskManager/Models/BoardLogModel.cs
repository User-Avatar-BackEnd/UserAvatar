using System;
using System.Collections.Concurrent;
using System.Linq;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.TaskManager.Models
{
    public class BoardLogModel
    {
        private readonly struct Log
        {
            public Log(int userId, long ticks)
            {
                UserId = userId;
                Ticks = ticks;
            }

            public int UserId { get;}

            public long Ticks { get; }
        }

        private readonly ConcurrentQueue<Log> _logs;

        private readonly long lifeTime = TimeSpan.FromMinutes(1).Ticks;

        private readonly IDateTimeProvider _dateTimeProvider;

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
            if (_dateTimeProvider.DateTimeUtcNowTicks() - lastCheck > lifeTime)
            {
                return true;
            }
            var logsSnapshot = _logs.ToArray().Reverse();
            foreach(var log in logsSnapshot)
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

            while(_logs.TryPeek(out log))
            {
                if (_dateTimeProvider.DateTimeUtcNowTicks() - log.Ticks < lifeTime)
                {
                    break;
                }
                _logs.TryDequeue(out log);
            }
        }

    }
}
