using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.TaskManager.Models;

public sealed class ColumnModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int BoardId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int Index { get; set; }

    public List<CardModel> Cards { get; set; }

    public int ModifiedBy { get; set; }
}
