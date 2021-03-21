using System;
using AutoMapper;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Contracts.Requests;
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

        public TaskModel CreateTask(AddTaskRequest addTaskRequest, int userId)
        {
            //var boardId = _taskStorage.GetBoardId(taskId);
            //if (_boardStorage.IsUsersBoard(userId, boardId)) return null;//isUserBoard

            if (string.IsNullOrEmpty(addTaskRequest.Title)) return null;
            if (addTaskRequest.ColumnId < 1) return null;

            if (_taskStorage.GetTasksCountInColumn(addTaskRequest.ColumnId) > 100) throw new Exception();

            var task = new Task
            {
                Title = addTaskRequest.Title,
                Description = addTaskRequest.Description,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsHidden = false,
                ColumnId = addTaskRequest.ColumnId,
                Priority = addTaskRequest.Priority,
                ResponsibleId=addTaskRequest.ResponsibleId
            };

            task = _taskStorage.Create(task);
            var taskModel = _mapper.Map<Task, TaskModel>(task);
            return taskModel;
        }

        public TaskModel GetById(int taskId, int userId)
        {
            var boardId = _taskStorage.GetBoardId(taskId);
            if (_boardStorage.IsUsersBoard(userId,boardId)) return null;

            var task = _taskStorage.GetById(taskId);

            if (task == null) return null;

            return _mapper.Map<Task, TaskModel>(task);
        }

        public void DeleteTask(int taskId, int userId)
        {
            var boardId = _taskStorage.GetBoardId(taskId);
            if (_boardStorage.IsUsersBoard(userId, boardId)) throw new Exception();

            _taskStorage.DeleteTask(taskId);
        }
    }
}
