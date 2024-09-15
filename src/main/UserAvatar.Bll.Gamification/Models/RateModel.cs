namespace UserAvatar.Bll.Gamification.Models;

public sealed class RateModel
{
    public int Id { get; set; }

    public int RatePosition { get; set; }

    public string Login { get; set; }

    public string Rank { get; set; }

    public int Score { get; set; }

    public bool IsCurrentPlayer { get; set; }
}
