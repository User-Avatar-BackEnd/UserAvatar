namespace UserAvatar.Api.Contracts.ViewModels
{
    public class RateDataVm
    {
        public int Id { get; set; }

        public int RatePosition { get; set; }

        public string Login { get; set; }

        public string Rank { get; set; }

        public int Score { get; set; }

        public bool IsCurrentPlayer { get; set; }
    }
}
