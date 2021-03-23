namespace UserAvatar.Api.Contracts.ViewModels
{
    public class InviteVm
    {
        public int Id { get; set; }
        
        public UserShortVm Inviter { get; set; }
        
        public BoardShortVm BoardShortVm { get; set; }
        
        public int Status { get; set; }
    }
}