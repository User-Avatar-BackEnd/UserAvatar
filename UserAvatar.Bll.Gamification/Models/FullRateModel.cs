using System.Collections.Generic;

namespace UserAvatar.Bll.Gamification.Models
{
    public class FullRateModel
    {
        public List<RateModel> TopUsers { get; set; }

        public List<RateModel> UnderTopUsers { get; set; }
    }
}
