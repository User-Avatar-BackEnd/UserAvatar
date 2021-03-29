using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Full rate ViewModel
    /// </summary>
    public class FullRateVm
    {
        /// <summary>
        /// List of top 10 users
        /// </summary>
        public List<RateDataVm> TopUsers { get; set; }

        /// <summary>
        /// List of under top 10 users
        /// </summary>
        public List<RateDataVm> UnderTopUsers { get; set; }
    }
}
