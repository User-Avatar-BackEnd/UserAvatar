﻿using System;
namespace UserAvatar.Api.Contracts.ViewModel
{
    public class CommentVm
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public bool Editable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}