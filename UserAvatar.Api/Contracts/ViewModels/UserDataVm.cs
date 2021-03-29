namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// User data ViewModel
    /// </summary>
    public class UserDataVm
    {
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Amount of this user invites
        /// </summary>
        public int InvitesAmount { get; set; }
        
        /// <summary>
        /// This user daily event
        /// </summary>
        public DailyEventVm DailyEvent { get; set; }

        /// <summary>
        /// User rank
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// Previous level score
        /// </summary>
        public int PreviousLevelScore { get; set; }

        /// <summary>
        /// Current user score
        /// </summary>
        public int CurrentScoreAmount { get; set; }

        /// <summary>
        /// Score to achieve next level
        /// </summary>
        public int NextLevelScore { get; set; }
    }
}
