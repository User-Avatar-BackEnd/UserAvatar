using System;

namespace UserAvatar.Bll.Gamification.Models;

public sealed class HistoryModel
{
    public string EventName { get; set; }

    public int Score { get; set; }

    public DateTimeOffset DateTime { get; set; }
}
