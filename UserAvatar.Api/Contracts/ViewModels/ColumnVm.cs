using System.Collections.Generic;
namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Column ViewModel
    /// </summary>
    public class ColumnVm
    {
        /// <summary>
        /// Column id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Column title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Order of column
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// List of cards in this column
        /// </summary>
        public List<CardShortVm> Cards { get; set; }
    }
}
