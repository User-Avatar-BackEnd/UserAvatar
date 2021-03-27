using System;
namespace UserAvatar.Bll.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset DateTimeUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }

        public long DateTimeUtcNowTicks()
        {
            return DateTimeOffset.UtcNow.Ticks;
        }
    }
}
