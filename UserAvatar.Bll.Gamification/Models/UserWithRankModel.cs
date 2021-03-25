namespace UserAvatar.Bll.Gamification.Models
{
    public class UserWithRankModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public int Score { get; set; }
        public string Rank { get; set; }
    }
}