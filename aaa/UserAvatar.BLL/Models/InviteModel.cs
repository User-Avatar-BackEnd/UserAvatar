﻿using System;
namespace UserAvatar.BLL.Models
{
    public class InviteModel
    {
        public int Id { get; set; }
        public UserModel Inviter { get; set; }
        public UserModel Invited { get; set; }
        public int Status { get; set; }
        public DateTime Issued { get; set; }
    }
}
