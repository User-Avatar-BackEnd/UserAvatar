using System;
namespace UserAvatar.Api.Contracts.ViewModels
{
    /// <summary>
    /// Comment ViewModel
    /// </summary>
    public class CommentVm
    {
        /// <summary>
        /// Comment id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of user that placed this comment
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Comment text
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Flag if this comment is editable
        /// </summary>
        public bool Editable { get; set; }
        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
    }
}
