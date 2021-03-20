using System;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public class ColumnStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public void Create(Column column, out bool success)
        {
            throw new NotImplementedException();
        }
    }
}