using System.Collections.Generic;

namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Full board viewmodel
    /// </summary>
    public class BoardVm
    {
        /// <summary>
        /// Board id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Flag if this user is owner of this board
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Board title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Members list of this board
        /// </summary>
        public List<UserShortVm> Members { get; set; }

        /// <summary>
        /// Columns list of this board
        /// </summary>
        public List<FullColumnVm> Columns { get; set; }
    }
}
