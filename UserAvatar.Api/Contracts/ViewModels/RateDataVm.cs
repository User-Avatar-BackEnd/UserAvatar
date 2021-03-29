namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Data rate ViewModel
    /// </summary>
    public class RateDataVm
    {
        /// <summary>
        /// Rate id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Rate position
        /// </summary>
        public int RatePosition { get; set; }

        /// <summary>
        /// Login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// User rank
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// User score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Flag if this is current player
        /// </summary>
        public bool IsCurrentPlayer { get; set; }
    }
}
