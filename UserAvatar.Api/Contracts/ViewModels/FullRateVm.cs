using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels
{
    public class FullRateVm
    {
        public List<RateDataVm> TopUsers { get; set; }

        public List<RateDataVm> UnderTopUsers { get; set; }
    }
}
