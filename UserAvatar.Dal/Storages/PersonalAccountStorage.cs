using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class PersonalAccountStorage : IPersonalAccountStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public PersonalAccountStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public void Update(User user)
        {
            _userAvatarContext.Entry(user).State = EntityState.Modified;

            _userAvatarContext.SaveChanges();
        }
    }
}