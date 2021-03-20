﻿using System.ComponentModel.DataAnnotations;

namespace UserAvatar.API.Contracts
{
    public class AuthRequest
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 5)]
        public string Password { get; set; }
    }
}