﻿using System;
using System.Collections.Generic;

namespace UserAvatar.Bll.Models
{
    public class ColumnModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public BoardModel Board { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int Index { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }
}
