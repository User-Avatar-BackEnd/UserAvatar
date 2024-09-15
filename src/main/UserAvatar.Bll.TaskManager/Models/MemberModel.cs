namespace UserAvatar.Bll.TaskManager.Models;

public sealed class MemberModel
{
    public int Id { get; set; }

    public UserModel User { get; set; }

    public string Rank { get; set; }
}
