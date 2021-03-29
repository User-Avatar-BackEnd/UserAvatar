namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Invite ViewModel
    /// </summary>
    public class InviteVm
    {
        /// <summary>
        /// Id of this Invite
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Short model of user inviter
        /// </summary>
        public UserShortVm Inviter { get; set; }
        
        /// <summary>
        /// Short model of board to be invited
        /// </summary>
        public BoardShortVm Board { get; set; }
    }
}