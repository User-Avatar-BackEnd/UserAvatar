using System.Collections.Generic;
namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Full column ViewModel
    /// </summary>
    public class FullColumnVm
    {
        /// <summary>
        /// Column id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Order of this column
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Title of this column
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// List of all cars in this column
        /// </summary>
        public List<CardVm> Cards { get; set; }
    }
}