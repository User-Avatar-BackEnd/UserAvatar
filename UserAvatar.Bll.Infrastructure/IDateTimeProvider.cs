using System;
namespace UserAvatar.Bll.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTimeOffset DateTimeUtcNow();

        long DateTimeUtcNowTicks();
    }
}
