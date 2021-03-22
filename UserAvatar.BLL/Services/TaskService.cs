using System;
using AutoMapper;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public TaskService(ITaskStorage taskStorage, IMapper mapper, IBoardStorage boardStorage)
        {
            _taskStorage = taskStorage;
            _boardStorage = boardStorage;
            _mapper = mapper;
        }

        public TaskModel CreateTask(string title, int columnId, int userId)
        {
            //var boardId = _taskStorage.GetBoardId(taskId);
            //if (_boardStorage.IsUsersBoard(userId, boardId)) return null;//isUserBoard

            if (string.IsNullOrEmpty(title)) return null;
            if (columnId < 1) return null;

            if (_taskStorage.GetTasksCountInColumn(columnId) > 100) throw new Exception();

            var task = new Task
            {
                Title = title,
                Description = "",
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsHidden = false,
                ColumnId = columnId
            };

            task = _taskStorage.Create(task);
            var taskModel = _mapper.Map<Task, TaskModel>(task);
            return taskModel;
        }

        public void UpdateTask(TaskModel taskModel, int columnId, int? responsibleId, int userId)
        {
            var boardId = _taskStorage.GetBoardId(taskModel.Id);
            if (_boardStorage.IsUserBoard(userId, boardId)) throw new Exception();

            var task = _taskStorage.GetById(taskModel.Id);

            task.Title = task.Title;
            task.Description = taskModel.Description;
            task.ColumnId = columnId;
            task.ResponsibleId = responsibleId;
            task.IsHidden = taskModel.IsHidden;
            task.ModifiedAt = DateTime.UtcNow;
            task.Priority = taskModel.Priority;

            _taskStorage.Update(task);
        }

        public TaskModel GetById(int taskId, int userId)
        {
            var boardId = _taskStorage.GetBoardId(taskId);
            if (_boardStorage.IsUserBoard(userId,boardId)) throw new Exception();

            var task = _taskStorage.GetById(taskId);

            if (task == null) return null;

            return _mapper.Map<Task, TaskModel>(task);
        }

        public void DeleteTask(int taskId, int userId)
        {
            var boardId = _taskStorage.GetBoardId(taskId);
            if (_boardStorage.IsUserBoard(userId, boardId)) throw new Exception();

            _taskStorage.Delete(taskId);
        }
    }
}
