namespace UserAvatar.Api.Contracts.ViewModels
{
    public class UserDataVm
    {
        public string Email { get; set; }

        public string Login { get; set; }

        public string Role { get; set; }

        public int InvitesAmount { get; set; }
        
        public DailyEventVm DailyEvent { get; set; }

        public string Rank { get; set; }

        public int PreviousLevelScore { get; set; }

        public int CurrentScoreAmount { get; set; }

        public int NextLevelScore { get; set; }
    }
}
