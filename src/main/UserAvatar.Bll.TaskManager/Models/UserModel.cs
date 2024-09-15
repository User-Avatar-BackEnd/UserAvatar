using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models;

public sealed class UserModel
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Login { get; set; }

    public string PasswordHash { get; set; }

    public int Score { get; set; }

    public string Role { get; set; }

    public List<BoardModel> Boards { get; set; }

    public List<CommentModel> Comments { get; set; }

    public List<InviteModel> Invited { get; set; }

    public List<InviteModel> Inviter { get; set; }
}

