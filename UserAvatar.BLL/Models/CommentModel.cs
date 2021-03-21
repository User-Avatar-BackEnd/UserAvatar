﻿using System;
namespace UserAvatar.Bll.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public TaskModel Task { get; set; }
        public UserModel User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
