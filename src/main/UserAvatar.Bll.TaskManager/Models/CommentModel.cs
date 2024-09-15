using System;

namespace UserAvatar.Bll.TaskManager.Models;

public sealed class CommentModel
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
}
