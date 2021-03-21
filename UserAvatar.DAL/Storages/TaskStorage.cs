using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages
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

        public Task Create(Task task)
        {
            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();
            return _dbContext.Tasks
                .Include(x => x.Column)
                .Include(x => x.Responsible)
                .Include(x => x.Comments)
                .First(x => x.Id == task.Id);
        }

        public int GetTasksCountInColumn(int columnId)
        {
            var column = _dbContext.Columns
                .Include(x => x.Tasks)
                .Where(x => x.Id == columnId &&!x.isDeleted)
                .FirstOrDefault();

            if (column == null) throw new Exception(); //column dosent exist

            return column.Tasks.Where(x => !x.IsDeleted).Count();
        }

        public int GetBoardId(int taskId)
        {
            return _dbContext.Tasks
                .Include(x => x.Column)
                .FirstOrDefault(x => x.Id == taskId)
                .Column.BoardId;
        }
    }
}
