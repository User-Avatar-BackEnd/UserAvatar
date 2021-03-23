﻿using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class CommentRequest
    {
        [Required]
        public int CardId { get; set; }
        
        [Required]
        //todo: add validator
        public string Text { get; set; }
    }
}