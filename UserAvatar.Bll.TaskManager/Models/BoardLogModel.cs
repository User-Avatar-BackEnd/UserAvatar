using System;
using System.Collections.Concurrent;
using System.Linq;

namespace UserAvatar.Bll.TaskManager.Models
{
    public class BoardLogModel
    {
        private readonly struct Log
        {
            public Log(int userId)
            {
                UserId = userId;
                Ticks = DateTimeOffset.UtcNow.Ticks;
            }

            public int UserId { get;}

            public long Ticks { get; }
        }

        private readonly ConcurrentQueue<Log> _logs;

        private readonly long lifeTime = TimeSpan.FromMinutes(1).Ticks;

        public BoardLogModel()
        {
            _logs = new ConcurrentQueue<Log>();
        }

        public void AddChange(int userId)
        {
            Clear();
            _logs.Enqueue(new Log(userId));
        }

        public bool HasChanges(int userId, long lastCheck)
        {
            Clear();
            if (DateTimeOffset.UtcNow.Ticks - lastCheck > lifeTime)
            {
                return true;
            }
            //_logs.ToArray().Reverse();
            foreach(var log in _logs.Reverse())
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
                if (DateTimeOffset.UtcNow.Ticks - log.Ticks < lifeTime)
                {
                    break;
                }
                _logs.TryDequeue(out log);
            }
        }

    }
}
