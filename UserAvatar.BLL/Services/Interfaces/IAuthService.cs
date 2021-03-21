﻿using System.Collections.Generic;
using UserAvatar.Bll.Models;

namespace UserAvatar.Bll.Services.Interfaces
{
    public interface IAuthService
    {
        public UserModel Register(string email, string login, string password);

        public UserModel Login(string email, string password);
    }
}