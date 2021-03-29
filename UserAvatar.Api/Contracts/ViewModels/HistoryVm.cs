using System;
namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// History ViewModel
    /// </summary>
    public class HistoryVm
    {
        /// <summary>
        /// Event name 
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Score of this event
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Issued DateTime
        /// </summary>
        public DateTimeOffset DateTime { get; set; }
    }
}
