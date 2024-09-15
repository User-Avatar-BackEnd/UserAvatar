using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models;

public sealed class BoardModel
{
    public int Id { get; init; }

    public string Title { get; init; }

    public int OwnerId { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset ModifiedAt { get; init; }

    public List<ColumnModel> Columns { get; init; }

    public List<MemberModel> Members { get; init; }

    public int ModifiedBy { get; init; }
}
