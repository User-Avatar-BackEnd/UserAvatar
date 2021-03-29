namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// User page data ViewModel
    /// </summary>
    public class UserPageDataVm
    {
        /// <summary>
        /// Position index
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// User rank
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// User score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public string Role { get; set; }
    }
}
