using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Storages
{
    public class TaskStorage
    {
        private readonly UserAvatarContext _dbContext;

        public TaskStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task GetById(int id)
        {
            return _dbContext.Tasks
                .Include(x=>x.Column)
                .Include(x=>x.Responsible)
                .Include(x=>x.Comments)
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }
    }
}
